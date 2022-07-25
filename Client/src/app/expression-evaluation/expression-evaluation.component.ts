import { Component, OnInit } from '@angular/core';
import { filterCmpinfo, filterStderr, validateExpression, wrapExpression } from '../languages/language-helpers';
import { JobeServerService } from '../services/jobe-server.service';
import { EmptyRunResult, getResultOutcome, RunResult } from '../services/run-result';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-expression-evaluation',
  templateUrl: './expression-evaluation.component.html',
  styleUrls: ['./expression-evaluation.component.css']
})
export class ExpressionEvaluationComponent implements OnInit {

  constructor(private jobeServer: JobeServerService, private route: ActivatedRoute) {
    this.result = EmptyRunResult;
  }

  ngOnInit(): void {
    this.jobeServer.get_languages().subscribe(supportedLanguages => {
      this.languages = supportedLanguages;
      this.checkLanguage()
    });
    this.route.queryParams.subscribe(params => {
      this.selectedLanguage = params['language'];
      this.checkLanguage()
    });
  }

  private checkLanguage() {
    if (this.selectedLanguage && this.languages.length > 0) {
      // check language is supported if not leave empty
      if (!this.languages.map(l => l[0]).includes(this.selectedLanguage)) {
        this.selectedLanguage = '';
      }
    }
  }

  previousExpressionIndex = 0;

  previousExpressions: [expr: string, result: string][] = [];

  expression: string = '';

  validationFail: string = ''

  result: RunResult;

  languages: Array<[string, string]> = [];

  selectedLanguage: string = '';

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
    if (this.expression != ``) {
      this.result = EmptyRunResult;
      this.validationFail = validateExpression(this.selectedLanguage, this.expression, []);
      if (!this.validationFail) {
        const code = wrapExpression(this.selectedLanguage, this.expression);
        this.jobeServer.submit_run(this.selectedLanguage, code).subscribe(rr => {
          this.result = rr;
          this.previousExpressions.push([this.expression, this.result.stdout.trim()]);
          this.expression = '';
          this.previousExpressionIndex = this.previousExpressions.length;
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
