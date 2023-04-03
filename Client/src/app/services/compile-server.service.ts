import { Injectable } from '@angular/core';
import { ActionResultRepresentation, DomainObjectRepresentation, DomainServicesRepresentation, IHateoasModel, InvokableActionMember, Value } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { BehaviorSubject, catchError, from, of, Subject } from 'rxjs';
import { RunResult, errorRunResult, EmptyRunResult } from '../models/run-result';
import { TaskService } from './task.service';
import { Dictionary } from 'lodash';
import { SubmitProgram } from 'armlite_service';
import { ArmTestHelper } from '../models/arm-test-helper';
import { ITaskUserView } from '../models/task-user-view';
import { ILanguageView, LanguageView } from '../models/language-view';


enum ActivityType {
  submitCodeFail, 
  submitCodeSuccess, 
  runTestsFail,
  runTestsSuccess,
  hintUsed
}

@Injectable({
  providedIn: 'root'
})
export class CompileServerService {

  
  constructor(taskService: TaskService, private repLoader: RepLoaderService, private contextService: ContextService) {
    taskService.currentTask.subscribe(t => {
      this.currentTask = t;
      this.selectedLanguage = t.Language;
    });

    this.getService();
  }

  private getService() {
    this.contextService.getServices()
        .then((services: DomainServicesRepresentation) => {
          const service = services.getService("Model.Functions.Services.Compile");
          return this.repLoader.populate(service);
        })
        .then((service : IHateoasModel) => {
          this.compileServer = service as DomainObjectRepresentation;
          this.getLanguages();
        });
      }

  private compileServer? : DomainObjectRepresentation;

  private currentTask? : ITaskUserView;

  selectedLanguage = '';

  languages$ = new BehaviorSubject<ILanguageView[]>([]);

  private userDefinedCode = '';

  lastExpressionResult = new Subject<RunResult>();

  // easier to test functions
  setUserDefinedCode(userCode: string) {
    this.userDefinedCode = userCode;
  }

  clearUserDefinedCode() {
    this.userDefinedCode = '';
  }

  hasUserDefinedCode() {
    return !!this.userDefinedCode;
  }

  private ToRunResult(ar: ActionResultRepresentation) : RunResult {
      const result = ar.result().object();

      return result ? {
        cmpinfo: result?.propertyMember("Cmpinfo").value().scalar(),
        outcome :  result?.propertyMember("Outcome").value().scalar(),
        stderr :  result?.propertyMember("Stderr").value().scalar(),
        stdout:  result?.propertyMember("Stdout").value().scalar(),
        run_id: result?.propertyMember("RunID").value().scalar(),
        lineno: result?.propertyMember("LineNo").value().scalar(),
        colno: result?.propertyMember("ColNo").value().scalar(),
      } as RunResult : errorRunResult({message : "null result from server"});
  }

  private ToLanguages(ar: ActionResultRepresentation): ILanguageView[] {
    const result = ar.result().list();
    const items = result?.value() || [];
    return items.map(l => ({ AlphaName: l.title() || "", Version: "" })).sort((a, b) => b.AlphaName.localeCompare(a.AlphaName));
  }

  // expose to make testing easier 

  get evaluateExpressionAction() {
    return this.compileServer!.actionMember("EvaluateExpression") as InvokableActionMember;
  }

  get submitCodeAction() {
    return this.compileServer!.actionMember("SubmitCode") as InvokableActionMember;
  }

  get runTestsAction() {
    return this.compileServer!.actionMember("RunTests") as InvokableActionMember;
  }

  get recordActivityAction() {
    return this.compileServer!.actionMember("RecordCodeActivityWithoutCompiling") as InvokableActionMember;
  }

  get getLanguagesAction() {
    return this.compileServer!.actionMember("GetLanguagesAndVersions") as InvokableActionMember;
  }

  get urlParams() {
    return {} as Dictionary<Object>;
  }

  private submit(action: InvokableActionMember, params: Dictionary<Value>) {
   
    return from(this.repLoader.invoke(action, params, this.urlParams)
      .then(ar => this.ToRunResult(ar)))
      .pipe(catchError((e) => of<RunResult>(errorRunResult(e))));
  }

  private submitVoid(rr : RunResult, action: InvokableActionMember, params: Dictionary<Value>) {
    return from(this.repLoader.invoke(action, params, this.urlParams)
      .then(ar => rr)
      .catch(e => rr));
  }

  params(taskId: number, expression?: string, code?: string) {
    const dict = { "taskId": new Value(taskId) } as Dictionary<Value>;
    if (expression) {
      dict["expression"] = new Value(expression);
    }
    dict["code"] = new Value(code || this.userDefinedCode);
    return dict;
  }

  activityParams(taskId: number, type: ActivityType, code: string) {
    const dict = {
      "taskId": new Value(taskId),
      "activityType": new Value(type),
      "code": new Value(code)
    } as Dictionary<Value>;

    return dict;
  }

  evaluateExpression(taskId: number, expression: string) {
    const action = this.evaluateExpressionAction;
    const params = this.params(taskId, expression);
    return this.submit(action, params);
  }

  recordActivity(rr: RunResult,  taskId: number, code: string, type: ActivityType) {
    const action = this.recordActivityAction;
    const params = this.activityParams(taskId, type, code);
    return this.submitVoid(rr, action, params);
  }

  private getCodeToSubmit(rr: RunResult, code: string) {
    if (rr.formattedsource) {
      const el = document.createElement("div");
      el.innerHTML = rr.formattedsource;
      return el.innerText.replace(/\xa0/g, ' ').replace(/(\s*\d+\|)/g, '\n$1').replace(/\n/, ''); // replace nbsp and add line breaks before each line except first
    }
    return code;
  }

  submitARMCode(taskId: number, code: string) {

    const rr = SubmitProgram(code) as RunResult;
    if (rr.outcome === 15) {
      rr.stdout = rr.cmpinfo;
      rr.cmpinfo = "";
      const codeToSubmit = this.getCodeToSubmit(rr, code);
      return this.recordActivity(rr, taskId, codeToSubmit, ActivityType.submitCodeSuccess);
    }
    else {
      return this.recordActivity(rr, taskId, code, ActivityType.submitCodeFail);
    }
  }

  submitCode(taskId: number, code: string) {
    if (this.selectedLanguage === "arm") {
      return this.submitARMCode(taskId, code);
    }

    const action = this.submitCodeAction;
    const params = this.params(taskId, undefined, code);
    return this.submit(action, params);
  }

  runLocalTests(taskId: number) {
    const rr = ArmTestHelper.runTests(this.currentTask?.ClientRunTestCode!, this.userDefinedCode) as RunResult;
    return this.recordActivity(rr, taskId, this.userDefinedCode, rr.outcome === 15 ? ActivityType.runTestsSuccess : ActivityType.runTestsFail);
  }

  runTests(taskId: number) {
    if (this.currentTask?.TestsRunLocally) {
      return this.runLocalTests(taskId);
    }

    const action = this.runTestsAction;
    const params = this.params(taskId);
    return this.submit(action, params);
  }

  getLanguages() {
    const action = this.getLanguagesAction;
    this.repLoader.invoke(action, {}, this.urlParams)
      .then(ar => this.languages$.next(this.ToLanguages(ar)));
  }
}
