import { Component, OnInit } from '@angular/core';
import { filterCmpinfo, validateExpression, wrapExpression } from '../language-helpers';
import { JobeServerService } from '../jobe-server.service';
import { getResultOutcome, RunResult } from '../run-result';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-expression-evaluation',
  templateUrl: './expression-evaluation.component.html',
  styleUrls: ['./expression-evaluation.component.css']
})
export class ExpressionEvaluationComponent implements OnInit {

  constructor(private jobeServer: JobeServerService, private route: ActivatedRoute) {
    this.result = jobeServer.emptyResult;
  }

  ngOnInit(): void {
    this.jobeServer.get_languages().subscribe(o => {
      this.languages = o.filter(i => this.jobeServer.supportedLanguages.includes(i[0]));
      this.checkLanguage()
    });
    this.route.queryParams.subscribe(params => {
      this.selectedLanguage = params['language'];
      this.checkLanguage()
    });
  }

  private checkLanguage() {
    if (this.selectedLanguage && this.languages.length > 0) {
      // check language is supported if not default
      if (!this.languages.map(l => l[0]).includes(this.selectedLanguage)) {
        this.selectedLanguage = this.languages[0][0];
      }
    }
  }


  previousExpressionIndex = 0;

  previousExpressions: [string, string][] = [];

  expression: string = '';

  whiteListFunctions: string = '';

  validationFail: string = ''

  result: RunResult;

  languages: Array<[string, string]> = [];

  selectedLanguage: string = 'csharp';

  filteredCmpinfo() {
    if (this.validationFail) {
      return this.validationFail;
    }

    return this.result.cmpinfo
      ? filterCmpinfo(this.selectedLanguage, this.result.cmpinfo)
      : this.result.stderr;
  }

  mapOutcome(outcome: number) {
    return getResultOutcome(outcome);
  }

  get editorDisplay() {
    let display = "";

    for (const e of this.previousExpressions) {
      display += `>${e[0]}\n`;
      display += `${e[1]}\n`;
    }
    return display;
  }

  onEnter() {
    this.expression = this.expression.trim();
    if (this.expression != ``) {
      this.result = this.jobeServer.emptyResult;
      const whiteList = this.whiteListFunctions.split(",").map(s => s.trim());
      this.validationFail = validateExpression(this.selectedLanguage, this.expression, whiteList);
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

  onClear() {
    this.previousExpressions = [];
    this.previousExpressionIndex = 0;
    this.result = this.jobeServer.emptyResult;
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
