import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { RunResult, errorRunResult } from './run-result';
import { FunctionPlaceholder, ReadyMadeFunctionsPlaceholder } from '../languages/language-helpers';
import { TaskService } from './task.service';

@Injectable({
  providedIn: 'root'
})
export class JobeServerService {

  constructor(private http: HttpClient, taskService: TaskService) {
    taskService.currentTask.subscribe(t => {
      this.readyMadeFunctions = t.ReadyMadeFunctions || '';
    })
  }

  private ip = "http://20.82.150.165";
  path = `${this.ip}/jobe/index.php/restapi`;

  selectedLanguage: string = '';

  private functionDefinitions: string = '';
  private readyMadeFunctions: string = '';

  // easier to test functions
  setFunctionDefinitions(functionDefinitions: string) {
    this.functionDefinitions = functionDefinitions;
  }

  clearFunctionDefinitions() {
    this.functionDefinitions = '';
  }

  hasFunctionDefinitions() {
    return !!this.functionDefinitions;
  }

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json; charset-utf-8' })
  };

  run_spec(code: string) {
    code = code.replace(FunctionPlaceholder, this.functionDefinitions)
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
