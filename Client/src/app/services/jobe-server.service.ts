import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { of } from 'rxjs';
import { catchError, first } from 'rxjs/operators';
import { RunResult, errorRunResult } from '../models/run-result';
import { UserDefinedFunctionPlaceholder, ReadyMadeFunctionsPlaceholder } from '../language-helpers/language-helpers';
import { TaskService } from './task.service';

@Injectable({
  providedIn: 'root'
})
export class JobeServerService {

  constructor(private http: HttpClient, taskService: TaskService) {
    taskService.currentTask.subscribe(t => {

      if (t.ReadyMadeFunctions) {
        taskService.getFile(t.ReadyMadeFunctions)
          .then(h => this.readyMadeFunctions = h)
          .catch(_ => this.readyMadeFunctions = '');
      }

      this.selectedLanguage = t.Language;
    })
  }

  private ip = "https://compile.metalup.org";
  path = `${this.ip}/jobe/index.php/restapi`;

  // private ip = "https://metalupcompileserver.azurewebsites.net";
  // path = `${this.ip}/restapi`;

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

  submit_run(code: string) {
    return this.http.post<RunResult>(`${this.path}/runs`, this.run_spec(code), this.httpOptions).pipe(catchError((e) => of<RunResult>(errorRunResult(e))));
  }

  get_languages() {
    return this.http.get<Array<[string, string]>>(`${this.path}/languages`, this.httpOptions).pipe(catchError(() => of<Array<[string, string]>>([])));
  }
}
