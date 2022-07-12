import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { RunResult } from './run-result';

@Injectable({
  providedIn: 'root'
})
export class JobeServerService {

  constructor(private http: HttpClient) { }

  errorResult : RunResult = {
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
    return {"run_spec": {"language_id": language,  "sourcecode": code}};
  }

  run(language : string, code : string){
      return this.http.post<RunResult>("http://20.82.150.165/jobe/index.php/restapi/runs", this.body(language, code), this.httpOptions).pipe(catchError(() => of<RunResult>(this.errorResult)));
  }

  get(){
    return this.http.get<Array<[string, string]>>("http://20.82.150.165/jobe/index.php/restapi/languages", this.httpOptions).pipe(catchError(() => of<Array<[string, string]>>([])));
  }

}
