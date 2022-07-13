import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TestRunnerService {

  private testString = `
     return function (expressionResult, result){
       return expressionResult.trim() === result ? 'Test passed' : expressionResult.toString() + " does not equal " + result.toString() ; 
     }`

  constructor() { }

  test(er: string) {
    var f = Function(this.testString);
    return f()(er, "4") as boolean;
  }
}
