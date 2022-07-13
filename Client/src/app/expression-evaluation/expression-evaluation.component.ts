import { Component, Input, OnInit } from '@angular/core';
import { filterCmpinfo, validateExpression, wrapExpression } from '../language-helpers';
import { JobeServerService } from '../jobe-server.service';
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
    this.validation = validateExpression(this.selectedLanguage, this.expression, whiteList);
    if (!this.validation) {
      const code = wrapExpression(this.selectedLanguage, this.expression);
      this.jobeServer.submit_run(this.selectedLanguage, code).subscribe(rr => this.result = rr);
    }
  }
}
