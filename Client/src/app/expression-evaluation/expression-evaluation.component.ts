import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { Applicability, ErrorType } from '../models/rules';
import { RulesService } from '../services/rules.service';
import { EmptyRunResult, getResultOutcome, RunResult } from '../models/run-result';
import { TaskService } from '../services/task.service';
import { first } from 'rxjs/operators';
import { CompileServerService } from '../services/compile-server.service';
import { nextExpressionEnabledTooltip, nextExpressionDisabledTooltip, previousExpressionEnabledTooltip, previousExpressionDisabledTooltip, submitExpressionDisabledTooltip, submitExpressionEnabledTooltip } from '../constants/tooltips';

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

  previousExpressionIndex = 0;

  previousExpressions: [expr: string, result: string][] = [];

  expression: string = '';

  validationFail: string = '';

  result: RunResult = EmptyRunResult;

  private canPaste = false;

  taskId = 0;

  disabled = true;

  get selectedLanguage() {
    return this.compileServer.selectedLanguage;
  }

  get nextExpressionTooltip() {
    return  this.canNext() ?  nextExpressionEnabledTooltip : nextExpressionDisabledTooltip;
  }

  get previousExpressionTooltip() {
    return  this.canPrevious() ?  previousExpressionEnabledTooltip : previousExpressionDisabledTooltip;
  }

  get submitExpressionTooltip() {
    return  this.canSubmit() ?  submitExpressionEnabledTooltip : submitExpressionDisabledTooltip;
  }

  filteredCmpinfo() {
    return this.result.cmpinfo
      ? this.rulesService.filter(ErrorType.cmpinfo, this.result.cmpinfo)
      : '';
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

  get expressionError() {
    return this.validationFail || this.filteredCmpinfo();
  }

  private pushExpression() {
    this.previousExpressions.push([this.expression, this.result.stdout]);
    this.previousExpressionIndex = this.previousExpressions.length -1;
  }

  canSubmit() {
    return this.expression.trim() !== "";
  }

  onSubmit() {
    this.expression = this.expression.trim();
    if (this.expression !== "") {
      this.result = EmptyRunResult;
      this.validationFail = this.rulesService.checkRules(Applicability.expressions, this.expression);
      if (!this.validationFail) {
        this.compileServer.evaluateExpression(this.taskId, this.expression).pipe(first()).subscribe(rr => {
          this.result = rr;
          this.pushExpression();
          this.compileServer.lastExpressionResult.next(rr);
        });
      }
      else {
        this.pushExpression();
      }
    }
  }

  onPaste(event: Event) {
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
      this.onPrevious();
    }
    if (event.key === "ArrowDown") {
      this.onNext();
    }
  }

  canPrevious() {
    return this.previousExpressionIndex > 0;
  }

  onPrevious() {
    this.previousExpressionIndex = this.previousExpressionIndex <= 0 ? 0 : this.previousExpressionIndex - 1;
    this.expression = this.previousExpressions[this.previousExpressionIndex][0].trim();
  }

  canNext() {
    return this.previousExpressionIndex < this.previousExpressions.length;
  }

  onNext() {
    if (this.previousExpressionIndex >= this.previousExpressions.length - 1) {
      this.previousExpressionIndex = this.previousExpressions.length;
      this.expression = '';
    }
    else {
      this.previousExpressionIndex = this.previousExpressionIndex + 1;
      this.expression = this.previousExpressions[this.previousExpressionIndex][0].trim();
    }
  }

  
  get paneSize() {
    return "pane-size-medium";
  }

  private sub?: Subscription;

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(t => {
      this.disabled = !t.Language;
      if (!this.disabled) {
        this.canPaste = t.PasteExpression;
        this.taskId = t.Id;
      }
    });
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
