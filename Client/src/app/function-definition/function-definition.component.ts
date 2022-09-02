import { Component, OnDestroy, OnInit } from '@angular/core';
import { wrapFunctions } from '../languages/language-helpers';
import { JobeServerService } from '../services/jobe-server.service';
import { Applicability, ErrorType } from '../services/rules';
import { RulesService } from '../services/rules.service';
import { EmptyRunResult, RunResult } from '../services/run-result';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-function-definition',
  templateUrl: './function-definition.component.html',
  styleUrls: ['./function-definition.component.css']
})
export class FunctionDefinitionComponent implements OnInit, OnDestroy {

  constructor(
    private jobeServer: JobeServerService,
    private rulesService: RulesService,
    private taskService: TaskService) {
  }

  result: RunResult = EmptyRunResult;

  compiledOK = false;

  functionDefinitions = '';

  pendingSubmit = false;

  submitting = false;

  skeleton = '';

  private canPaste = false;

  nextTaskClears = true;

  get skeletonUnchanged() {
    return this.functionDefinitions === this.skeleton;
  };

  validationFail: string = '';

  get currentStatus() {
    return this.validationFail ||
      this.rulesService.filter(this.jobeServer.selectedLanguage, ErrorType.cmpinfo, this.result.cmpinfo) ||
      this.rulesService.filter(this.jobeServer.selectedLanguage, ErrorType.stderr, this.result.stderr) ||
      (this.compiledOK ? 'Compiled OK' : '');
  }

  modelChanged() {
    this.validationFail = '';
    this.compiledOK = false;
    this.result = EmptyRunResult;
    this.pendingSubmit = !!(this.functionDefinitions.trim());
    this.jobeServer.clearFunctionDefinitions();
  }

  onPaste(event: ClipboardEvent) {
    if (!this.canPaste) {
      event.preventDefault();
    }
  }

  onReset() {
    this.functionDefinitions = this.skeleton;
    this.modelChanged();
  }

  onSubmit() {
    this.compiledOK = false;
    this.pendingSubmit = false;
    this.validationFail = this.rulesService.checkRules(this.jobeServer.selectedLanguage, Applicability.functions, this.functionDefinitions);
    if (!this.validationFail) {
      this.submitting = true;
      const code = wrapFunctions(this.jobeServer.selectedLanguage, this.functionDefinitions);
      this.jobeServer.submit_run(code).pipe(first()).subscribe(rr => {
        this.result = rr;
        this.compiledOK = !(this.result.cmpinfo || this.result.stderr) && this.result.outcome == 15;
        if (this.compiledOK) {
          this.jobeServer.setFunctionDefinitions(this.functionDefinitions);
        }
        this.submitting = false;
      });
    }
  }

  private placeholderMap: Map<string, string> = new Map(
    [
      ['csharp', 'static <returnType> Name(<parameter definitions>) => <expression>;'],
      ['python', 'def name(<parameter definitions>) : return <expression>']
    ]);
    
  get placeholder() {
    return this.placeholderMap.get(this.jobeServer.selectedLanguage) || '';
  }

  private sub?: Subscription;

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(t => {
      this.canPaste = !!t.PasteFunction;
      this.skeleton = t.SkeletonCode || '';
      
      if (this.nextTaskClears) {
        this.functionDefinitions = this.skeleton;
        this.modelChanged();
      }

      this.nextTaskClears = !!!t.NextTaskDoesNotClearFunctions;
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
