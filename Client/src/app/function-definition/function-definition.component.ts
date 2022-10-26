import { Component, OnDestroy, OnInit } from '@angular/core';
import { wrapFunctions } from '../language-helpers/language-helpers';
import { Applicability, ErrorType } from '../models/rules';
import { RulesService } from '../services/rules.service';
import { EmptyRunResult, RunResult } from '../models/run-result';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { CompileServerService } from '../services/compile-server.service';

@Component({
  selector: 'app-function-definition',
  templateUrl: './function-definition.component.html',
  styleUrls: ['./function-definition.component.css']
})
export class FunctionDefinitionComponent implements OnInit, OnDestroy {

  constructor(
    private compileServer: CompileServerService,
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
      this.rulesService.filter(this.compileServer.selectedLanguage, ErrorType.cmpinfo, this.result.cmpinfo) ||
      this.rulesService.filter(this.compileServer.selectedLanguage, ErrorType.stderr, this.result.stderr) ||
      (this.compiledOK ? 'Compiled OK' : '');
  }

  modelChanged() {
    this.validationFail = '';
    this.compiledOK = false;
    this.result = EmptyRunResult;
    this.pendingSubmit = !!(this.functionDefinitions.trim());
    this.compileServer.clearFunctionDefinitions();
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
    this.validationFail = this.rulesService.checkRules(this.compileServer.selectedLanguage, Applicability.functions, this.functionDefinitions);
    if (!this.validationFail) {
      this.submitting = true;
      const code = wrapFunctions(this.compileServer.selectedLanguage, this.functionDefinitions);
      this.compileServer.submit_run(code, true).pipe(first()).subscribe(rr => {
        this.result = rr;
        this.compiledOK = !(this.result.cmpinfo || this.result.stderr) && this.result.outcome == 15;
        if (this.compiledOK) {
          this.compileServer.setFunctionDefinitions(this.functionDefinitions);
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
    return this.placeholderMap.get(this.compileServer.selectedLanguage) || '';
  }

  private sub?: Subscription;

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(t => {
      this.canPaste = !!t.PasteFunctions;
      this.skeleton = t.SkeletonCode || '';
      
      if (this.nextTaskClears) {
        this.functionDefinitions = this.skeleton;
        this.modelChanged();
      }

      this.nextTaskClears = !!t.NextTaskClearsFunctions;
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
