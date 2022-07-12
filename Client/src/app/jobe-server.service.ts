import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { RunResult } from './run-result';
import { wrap } from './language-helpers';

@Injectable({
  providedIn: 'root'
})
export class JobeServerService {

  constructor(private http: HttpClient) { }

  private ip = "http://20.82.150.165";
  private path = `${this.ip}/jobe/index.php/restapi`;

  // this is temporary - remove unsupported languages from jobe server
  supportedLanguages = ['csharp', 'java', 'python3', 'vbnet'];

  emptyResult : RunResult = {
    run_id : '',
    outcome : 0,
    cmpinfo : '', 
    stdout : '',
    stderr : ''
  }

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json; charset-utf-8' })
  };

  body(language : string, code : string) {
    //return {"run_spec": {"language_id": "c", "sourcefilename": "test.c", "sourcecode": "\n#include <stdio.h>\n\nint main() {\n    printf(\"Hello world\\n\");\n}\n"}};
    return {"run_spec": {"language_id": language,  "sourcecode": wrap(language, code)}};
  }

  run(language : string, code : string){
      return this.http.post<RunResult>(`${this.path}/runs`, this.body(language, code), this.httpOptions).pipe(catchError(() => of<RunResult>(this.emptyResult)));
  }

  get(){
    return this.http.get<Array<[string, string]>>(`${this.path}/languages`, this.httpOptions).pipe(catchError(() => of<Array<[string, string]>>([])));
  }

}
