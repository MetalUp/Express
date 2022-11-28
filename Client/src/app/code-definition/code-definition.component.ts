import { Component, OnDestroy, OnInit } from '@angular/core';
import { Applicability, ErrorType } from '../models/rules';
import { RulesService } from '../services/rules.service';
import { EmptyRunResult, RunResult } from '../models/run-result';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { CompileServerService } from '../services/compile-server.service';


@Component({
  selector: 'app-code-definition',
  templateUrl: './code-definition.component.html',
  styleUrls: ['./code-definition.component.css']
})
export class CodeDefinitionComponent implements OnInit, OnDestroy {

  constructor(
    private compileServer: CompileServerService,
    private rulesService: RulesService,
    private taskService: TaskService) {
  }

  result: RunResult = EmptyRunResult;

  compiledOK = false;

  codeDefinitions = '';

  unsubmittedCode = '';

  codeIndex = -1;

  hasPreviousCodeVersion = false;

  pendingSubmit = false;

  private canPaste = false;

  taskId = 0;

  validationFail: string = '';

  get currentStatus() {
    return this.validationFail ||
      this.rulesService.filter(ErrorType.cmpinfo, this.result.cmpinfo) ||
      this.rulesService.filter(ErrorType.stderr, this.result.stderr) ||
      (this.compiledOK ? 'Compiled OK' : '');
  }

  codeUpdated() {
    this.validationFail = '';
    this.compiledOK = false;
    this.result = EmptyRunResult;
    this.pendingSubmit = !!(this.codeDefinitions.trim());
    this.compileServer.clearUserDefinedCode();
  }

  modelChanged() {
    this.codeUpdated();
    this.unsubmittedCode = this.codeDefinitions;
    this.codeIndex = -1;
  }

  onPaste(event: ClipboardEvent) {
    if (!this.canPaste) {
      event.preventDefault();
    }
  }

  onSubmit() {
    this.codeIndex = -1;
    this.compiledOK = false;
    this.pendingSubmit = false;
    this.validationFail = this.rulesService.checkRules(Applicability.functions, this.codeDefinitions);
    if (!this.validationFail) {
      this.compileServer.submitCode(this.taskId, this.codeDefinitions).pipe(first()).subscribe(rr => {
        this.result = rr;
        this.compiledOK = !(this.result.cmpinfo || this.result.stderr) && this.result.outcome == 15;
        if (this.compiledOK) {
          this.compileServer.setUserDefinedCode(this.codeDefinitions);
          this.codeIndex = 0;
          this.unsubmittedCode = "";
        }
      });
    }
  }

  loadServerCode() {
    this.taskService.loadCode(this.taskId, this.codeIndex).then(c => {
      if (c.Code) {
        this.codeDefinitions = c.Code;
        this.hasPreviousCodeVersion = c.HasPreviousVersion;
        this.codeIndex = c.Version;
        this.codeUpdated();
      }
    });
  }

  canNextCode() {
    return this.codeIndex > 0  || (this.codeIndex == 0 && this.unsubmittedCode.trim() !== "");
  }

  nextCode() {
    if (this.codeIndex == 0) {
      // go to last submitted code
      this.codeIndex--;
      this.hasPreviousCodeVersion = true;
      this.codeDefinitions = this.unsubmittedCode;
      this.codeUpdated();
    }
    else {
      this.codeIndex--;
      this.loadServerCode();
    }
  }

  canPreviousCode() {
    return this.hasPreviousCodeVersion;
  }

  previousCode() {
    this.codeIndex++;
    this.loadServerCode();
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
      if (t.Id !== this.taskId) {
        this.codeDefinitions = t.Code || "";
        this.taskId = t.Id;
        this.canPaste = t.PasteCode;
        this.modelChanged();
        this.taskService.loadCode(this.taskId, 0).then(c => this.hasPreviousCodeVersion = c.HasPreviousVersion);
      }
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
