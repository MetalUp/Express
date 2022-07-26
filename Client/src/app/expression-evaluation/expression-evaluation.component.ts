import { Component } from '@angular/core';
import { filterCmpinfo, filterStderr, validateExpression, wrapExpression } from '../languages/language-helpers';
import { JobeServerService } from '../services/jobe-server.service';
import { RulesService, Applicability } from '../services/rules.service';
import { EmptyRunResult, getResultOutcome, RunResult } from '../services/run-result';

@Component({
  selector: 'app-expression-evaluation',
  templateUrl: './expression-evaluation.component.html',
  styleUrls: ['./expression-evaluation.component.css']
})
export class ExpressionEvaluationComponent {

  constructor(private jobeServer: JobeServerService, private rulesService: RulesService) {
    this.result = EmptyRunResult;
  }

  submitting = false;

  previousExpressionIndex = 0;

  previousExpressions: [expr: string, result: string][] = [];

  expression: string = '';

  validationFail: string = ''

  result: RunResult;

  get selectedLanguage() {
    return this.jobeServer.selectedLanguage;
  }

  filteredCmpinfo() {
    if (this.validationFail) {
      return this.validationFail;
    }

    return this.result.cmpinfo
      ? filterCmpinfo(this.selectedLanguage, this.result.cmpinfo)
      : filterStderr(this.selectedLanguage, this.result.stderr);
  }

  mapOutcome(outcome: number) {
    return getResultOutcome(outcome);
  }

  private getPrevious(i : number) {
    if (this.previousExpressions.length > 0) {
      const lastItem = this.previousExpressions[this.previousExpressions.length -1];
      return `${lastItem[i]}`;
    }
    return '';
  }

  get previousExpression() {
    return this.getPrevious(0);
  }

  get previousExpressionResult() {
    return this.getPrevious(1) || this.filteredCmpinfo();
  }

  onEnter() {
    this.expression = this.expression.trim();
    if (this.expression != "") {
      this.result = EmptyRunResult;
      this.validationFail = this.rulesService.validate(this.selectedLanguage, Applicability.expressions, this.expression);
      if (!this.validationFail) {
        this.submitting = true;
        const code = wrapExpression(this.selectedLanguage, this.expression);
        this.jobeServer.submit_run(code).subscribe(rr => {
          this.result = rr;
          this.previousExpressions.push([this.expression, this.result.stdout.trim()]);
          this.expression = '';
          this.previousExpressionIndex = this.previousExpressions.length;
          this.submitting = false;
        });
      }
    }
  }

  onEnterKey(event: KeyboardEvent) {
    if (event.key === "Enter") {
      event.preventDefault();
    }
  }

  onKey(event: KeyboardEvent) {
    if (event.key === "ArrowUp") {
      this.onUp();
    }
    if (event.key === "ArrowDown") {
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
