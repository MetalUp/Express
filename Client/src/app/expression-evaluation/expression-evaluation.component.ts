import { Component, Input, OnInit } from '@angular/core';
import { filterCmpinfo, validateExpression, wrapExpression } from '../language-helpers';
import { JobeServerService } from '../jobe-server.service';
import { TestRunnerService } from '../test-runner.service';
import { getResultOutcome, RunResult } from '../run-result';

@Component({
  selector: 'app-expression-evaluation',
  templateUrl: './expression-evaluation.component.html',
  styleUrls: ['./expression-evaluation.component.css']
})
export class ExpressionEvaluationComponent implements OnInit {

  constructor(private jobeServer: JobeServerService) {
    this.result = jobeServer.emptyResult;
  }

  ngOnInit(): void {
    this.jobeServer.get_languages().subscribe(o => this.languages = o.filter(i => this.jobeServer.supportedLanguages.includes(i[0])));
  }

  previousExpressionIndex = 0;

  previousExpressions: [string, string][] = [];

  expression: string = '';

  whiteListFunctions: string = '';

  validation: string = ''

  result: RunResult;

  languages: Array<[string, string]> = [];

  selectedLanguage: string = 'csharp';

  filteredCmpinfo() {
     return this.result.cmpinfo ? filterCmpinfo(this.selectedLanguage, this.result.cmpinfo) : '';
  }

  mapOutcome(outcome: number) {
    return getResultOutcome(outcome);
  }

  onEnter() {
    this.result = this.jobeServer.emptyResult;
    const whiteList = this.whiteListFunctions.split(",").map(s => s.trim());
    this.validation = validateExpression(this.selectedLanguage, this.expression.trim(), whiteList);
    if (!this.validation) {
      const code = wrapExpression(this.selectedLanguage, this.expression);
      this.jobeServer.submit_run(this.selectedLanguage, code).subscribe(rr => {
        this.result = rr;
        this.previousExpressions.push([this.expression, this.result.stdout]);
        this.expression = '';
        this.previousExpressionIndex = this.previousExpressions.length;
      });
    }    
  }

  onClear() {
    this.previousExpressions = [];
    this.previousExpressionIndex = 0;
    this.result = this.jobeServer.emptyResult;
  }

  onKey(event : KeyboardEvent) {
    if (event.key === "ArrowUp"){
      event.preventDefault();
      event.stopPropagation();
      this.onUp();
    }
    if (event.key === "ArrowDown"){
      event.preventDefault();
      event.stopPropagation();
      this.onDown();
    }
  }

  onUp() {
    this.previousExpressionIndex = this.previousExpressionIndex <= 0 ? 0 : this.previousExpressionIndex - 1;
    this.expression = this.previousExpressions[this.previousExpressionIndex][0].trim();
  }

  onDown() {
    if (this.previousExpressionIndex >= this.previousExpressions.length - 1) {
      this.previousExpressionIndex = this.previousExpressions.length;
      this.expression = '';
    }
    else {
      this.previousExpressionIndex = this.previousExpressionIndex + 1;
      this.expression = this.previousExpressions[this.previousExpressionIndex][0].trim();
    }
  }
}
