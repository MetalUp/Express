import { Component, Input, OnInit } from '@angular/core';
import { validateExpression } from '../language-helpers';
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

  validation: string = ''

  result: RunResult;

  languages: Array<[string, string]> = [];

  selectedLanguage: string = 'csharp';

  mapOutcome(outcome: number) {
    return getResultOutcome(outcome);
  }

  onEnter() {
    this.result = this.jobeServer.emptyResult;
    this.validation = '';
    if (validateExpression(this.selectedLanguage, this.expression)) {
      this.jobeServer.submit_run(this.selectedLanguage, this.expression).subscribe(o => this.result = o);
    }
    else {
      this.validation = `${this.expression} is not an expression`;
    }
  }
}
