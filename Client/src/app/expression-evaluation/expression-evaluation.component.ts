import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { wrapExpression } from '../language-helpers/language-helpers';
import { Applicability, ErrorType } from '../models/rules';
import { RulesService } from '../services/rules.service';
import { EmptyRunResult, getResultOutcome, RunResult } from '../models/run-result';
import { TaskService } from '../services/task.service';
import { first } from 'rxjs/operators';
import { CompileServerService } from '../services/compile-server.service';

@Component({
  selector: 'app-expression-evaluation',
  templateUrl: './expression-evaluation.component.html',
  styleUrls: ['./expression-evaluation.component.css']
})
export class ExpressionEvaluationComponent implements OnInit, OnDestroy {

  constructor(
    private compileServer: CompileServerService,
    private rulesService: RulesService,
    private taskService: TaskService) {
  }

  submitting = false;

  previousExpressionIndex = 0;

  previousExpressions: [expr: string, result: string][] = [];

  expression: string = '';

  validationFail: string = ''

  result: RunResult = EmptyRunResult;

  private canPaste = false;

  get selectedLanguage() {
    return this.compileServer.selectedLanguage;
  }

  filteredCmpinfo() {
    return this.result.cmpinfo
      ? this.rulesService.filter(this.selectedLanguage, ErrorType.cmpinfo, this.result.cmpinfo)
      : ''
  }

  filteredStderr() {
    return this.result.stderr
      ? this.rulesService.filter(this.selectedLanguage, ErrorType.stderr, this.result.stderr)
      : ''
  }

  mapOutcome(outcome: number) {
    return getResultOutcome(outcome);
  }

  private getPrevious(i: number) {
    if (this.previousExpressions.length > 0) {
      const lastItem = this.previousExpressions[this.previousExpressions.length - 1];
      return `${lastItem[i]}`;
    }
    return '';
  }

  get previousExpression() {
    return this.getPrevious(0);
  }

  get previousExpressionResult() {
    return this.getPrevious(1) || this.filteredStderr();
  }

  get expressionError() {
    return this.validationFail || this.filteredCmpinfo();
  }

  private pushExpression() {
    this.previousExpressions.push([this.expression, this.result.stdout]);
    this.previousExpressionIndex = this.previousExpressions.length -1;
  }

  onEnter() {
    this.expression = this.expression.trim();
    if (this.expression !== "") {
      this.result = EmptyRunResult;
      this.validationFail = this.rulesService.checkRules(this.selectedLanguage, Applicability.expressions, this.expression);
      if (!this.validationFail) {
        this.submitting = true;
        const code = wrapExpression(this.selectedLanguage, this.expression);
        this.compileServer.submit_run(code).pipe(first()).subscribe(rr => {
          this.result = rr;
          this.pushExpression();
          this.submitting = false;
        });
      }
      else {
        this.pushExpression();
      }
    }
  }

  onPaste(event: ClipboardEvent) {
    if (!this.canPaste) {
      event.preventDefault();
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

  private sub?: Subscription;

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(t => {
      this.canPaste = !!t.PasteExpression;
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
