import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActionResultRepresentation, DomainObjectRepresentation, DomainServicesRepresentation, IHateoasModel, InvokableActionMember, Value } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { catchError, from, of } from 'rxjs';
import { UserDefinedFunctionPlaceholder, ReadyMadeFunctionsPlaceholder } from '../language-helpers/language-helpers';
import { RunResult, errorRunResult, EmptyRunResult } from '../models/run-result';
import { TaskService } from './task.service';
import { Dictionary } from 'lodash';

@Injectable({
  providedIn: 'root'
})
export class CompileServerService {

  constructor(private http: HttpClient, taskService: TaskService, private repLoader: RepLoaderService, private contextService: ContextService) {
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

  private ip = "https://metalupcompileserver.azurewebsites.net";
  path = `${this.ip}/restapi`;

  selectedLanguage: string = '';

  private userDefinedFunction: string = '';
  private readyMadeFunctions: string = '';

  private programFileExtensions = ['.cs', '.py', '.txt'];

  private isProgramFile(fn: string) {
    return this.programFileExtensions.some(ext => fn.endsWith(ext) );
  }

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

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json; charset-utf-8' })
  };

  run_spec(code: string) {
    code = code.replace(UserDefinedFunctionPlaceholder, this.userDefinedFunction)
               .replace(ReadyMadeFunctionsPlaceholder, this.readyMadeFunctions);
    return { "run_spec": { "language_id": this.selectedLanguage, "sourcecode": code } };
  }

  private ToRunResult(ar: ActionResultRepresentation) : RunResult {
    return EmptyRunResult;
  }

  submit_run(code: string) {
    //return this.http.post<RunResult>(`${this.path}/runs`, this.run_spec(code), this.httpOptions).pipe(catchError((e) => of<RunResult>(errorRunResult(e))));

    //this.repLoader.invoke()

    const action = this.compileServer!.actionMember("Runs") as InvokableActionMember;

    return from(this.repLoader.invoke(action, {} as Dictionary<Value>, {} as Dictionary<Object>).then((ar) => 
       this.ToRunResult(ar)
    ))}
}
