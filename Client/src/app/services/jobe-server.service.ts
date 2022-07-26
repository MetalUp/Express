import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { RunResult, EmptyRunResult, errorRunResult } from './run-result';
import { FunctionPlaceholder } from '../languages/language-helpers';

@Injectable({
  providedIn: 'root'
})
export class JobeServerService {

  constructor(private http: HttpClient) { }

  private ip = "http://20.82.150.165";
  path = `${this.ip}/jobe/index.php/restapi`;

  selectedLanguage: string = '';

  private functionDefinitions: string = '';

  // easier to test functions
  setFunctionDefinitions(functionDefinitions : string) {
    this.functionDefinitions = functionDefinitions;
  }

  clearFunctionDefinitions() {
    this.functionDefinitions = '';
  }

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json; charset-utf-8' })
  };

  run_spec(code: string) {
    code = code.replace(FunctionPlaceholder, this.functionDefinitions);
    return { "run_spec": { "language_id": this.selectedLanguage, "sourcecode": code } };
  }

  submit_run(code: string) {
    return this.http.post<RunResult>(`${this.path}/runs`, this.run_spec(code), this.httpOptions).pipe(catchError((e) => of<RunResult>(errorRunResult(e))));
  }

  get_languages() {
    return this.http.get<Array<[string, string]>>(`${this.path}/languages`, this.httpOptions).pipe(catchError(() => of<Array<[string, string]>>([])));
  }
}
