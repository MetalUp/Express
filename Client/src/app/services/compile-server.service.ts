import { Injectable } from '@angular/core';
import { ActionResultRepresentation, DomainObjectRepresentation, DomainServicesRepresentation, IHateoasModel, InvokableActionMember, Value } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { catchError, from, of } from 'rxjs';
import { UserDefinedFunctionPlaceholder, ReadyMadeFunctionsPlaceholder } from '../language-helpers/language-helpers';
import { RunResult, errorRunResult } from '../models/run-result';
import { TaskService } from './task.service';
import { Dictionary } from 'lodash';

@Injectable({
  providedIn: 'root'
})
export class CompileServerService {

  constructor(taskService: TaskService, private repLoader: RepLoaderService, private contextService: ContextService) {
    taskService.currentTask.subscribe(t => {

      if (t.ReadyMadeFunctions) {
        taskService.getFile(t.ReadyMadeFunctions)
          .then(h => this.readyMadeFunctions = h)
          .catch(_ => this.readyMadeFunctions = '');
      }

      this.selectedLanguage = t.Language;
    })

    this.getService();
  }

  private getService() {
    this.contextService.getServices()
        .then((services: DomainServicesRepresentation) => {
          const service = services.getService("Model");
          return this.repLoader.populate(service);
        })
        .then((service : IHateoasModel) => {
          this.compileServer = service as DomainObjectRepresentation;
        })
      }

  private compileServer? : DomainObjectRepresentation;

  selectedLanguage: string = '';

  private userDefinedFunction: string = '';
  private readyMadeFunctions: string = '';

  // easier to test functions
  setFunctionDefinitions(functionDefinitions: string) {
    this.userDefinedFunction = functionDefinitions;
  }

  clearFunctionDefinitions() {
    this.userDefinedFunction = '';
  }

  hasFunctionDefinitions() {
    return !!this.userDefinedFunction;
  }

  run_spec(code: string) {
    code = code.replace(UserDefinedFunctionPlaceholder, this.userDefinedFunction)
               .replace(ReadyMadeFunctionsPlaceholder, this.readyMadeFunctions);
    return { "languageID": new Value(this.selectedLanguage), "code": new Value(code) } as Dictionary<Value>;
  }

  private ToRunResult(ar: ActionResultRepresentation) : RunResult {
      var result = ar.result().object();

      return result ? {
        cmpinfo: result?.propertyMember("Cmpinfo").value().scalar(),
        outcome :  result?.propertyMember("Outcome").value().scalar(),
        stderr :  result?.propertyMember("Stderr").value().scalar(),
        stdout:  result?.propertyMember("Stdout").value().scalar(),
        run_id: result?.propertyMember("RunID").value().scalar(),
      } as RunResult : errorRunResult({message : "null result from server"});
  }

  // expose to make testing easier 

  get runAction() {
    return this.compileServer!.actionMember("Runs") as InvokableActionMember;
  }

  get testAction() {
    return this.compileServer!.actionMember("Tests") as InvokableActionMember;
  }

  get evaluateExpressionAction() {
    return this.compileServer!.actionMember("EvaluateExpression") as InvokableActionMember;
  }

  get submitCodeAction() {
    return this.compileServer!.actionMember("SubmitCode") as InvokableActionMember;
  }

  get runTestsAction() {
    return this.compileServer!.actionMember("RunTests") as InvokableActionMember;
  }


  params1(code : string) {
    return this.run_spec(code);
  }

  get urlParams() {
    return {} as Dictionary<Object>;
  }

  submit_run(code: string, run: boolean) {
    const action = run ? this.runAction : this.testAction;

    return from(this.repLoader.invoke(action, this.params1(code), this.urlParams)
      .then(ar => this.ToRunResult(ar)))
      .pipe(catchError((e) => of<RunResult>(errorRunResult(e))));
  }

  private submit(action: InvokableActionMember, params: Dictionary<Value>) {
    return from(this.repLoader.invoke(action, params, this.urlParams)
      .then(ar => this.ToRunResult(ar)))
      .pipe(catchError((e) => of<RunResult>(errorRunResult(e))));
  }

  params(taskId: number, expression?: string, code?: string) {
    const dict = { "taskId": new Value(taskId) } as Dictionary<Value>;

    if (expression) {
      dict["expression"] = new Value(expression);
    }

    dict["code"] = new Value(code || this.userDefinedFunction);

    return dict;
  }

  evaluateExpression(taskId: number, expression: string) {
    const action = this.evaluateExpressionAction;
    var params = this.params(taskId, expression);
    return this.submit(action, params);
  }

  submitCode(taskId: number, code: string) {
    const action = this.submitCodeAction;
    var params = this.params(taskId, undefined, code);
    return this.submit(action, params);
  }

  runTests(taskId: number) {
    const action = this.runTestsAction;
    var params = this.params(taskId);
    return this.submit(action, params);
  }
}
