import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Applicability, ErrorType } from '../models/rules';
import { RulesService } from '../services/rules.service';
import { EmptyRunResult, RunResult } from '../models/run-result';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { CompileServerService } from '../services/compile-server.service';
import { EmptyCodeUserView, ICodeUserView } from '../models/code-user-view';
import { nextCodeEnabledTooltip, nextCodeDisabledTooltip, previousCodeEnabledTooltip, previousCodeDisabledTooltip, submitCodeEnabledTooltip, submitCodeDisabledTooltip, runAppEnabledTooltip, runAppDisabledTooltip } from '../constants/tooltips';

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

  currentCodeVersion: ICodeUserView = EmptyCodeUserView;

  pendingSubmit = false;

  private canPaste = false;

  taskId = 0;

  validationFail: string = '';


  get nextCodeTooltip() {
    return  this.canNewerCode() ?  nextCodeEnabledTooltip : nextCodeDisabledTooltip;
  }

  get previousCodeTooltip() {
    return  this.canOlderCode() ?  previousCodeEnabledTooltip : previousCodeDisabledTooltip;
  }

  get submitCodeTooltip() {
    return this.pendingSubmit ? submitCodeEnabledTooltip : submitCodeDisabledTooltip;
  }

  get RunAppTooltip() {
    return this.canRunApp ? runAppEnabledTooltip : runAppDisabledTooltip;
  }

  get currentStatus() {
    return this.validationFail ||
      this.rulesService.filter(ErrorType.cmpinfo, this.result.cmpinfo) ||
      this.rulesService.filter(ErrorType.stderr, this.result.stderr) ||
      (this.compiledOK ? 'Compiled OK' : '');
  }

  get errorLocation() {
    return (this.compiledOK || this.validationFail || !this.result.lineno) ? '' : ` (${this.result.lineno},${this.result.colno})`;
  }

  codeUpdated() {
    this.validationFail = '';
    this.compiledOK = false;
    this.result = EmptyRunResult;
    this.pendingSubmit = !!(this.codeDefinitions.trim());
    this.compileServer.clearUserDefinedCode();
  }

  modelChanged() {
    this.unsubmittedCode = this.codeDefinitions;
    this.setCodeVersion(this.getUnsubmittedCodeVersion(), true);
    this.refreshCursorPosition();
  }

  setCodeVersion(version: ICodeUserView, update: boolean) {
    this.currentCodeVersion = version;
    this.codeDefinitions = version.Code;
    if (update) {
      this.codeUpdated();
    }
  }

  onPaste(event: Event) {
    if (!this.canPaste) {
      event.preventDefault();
    }
  }

  get displayRunApp() {
    return this.compileServer.selectedLanguage === "arm";
  }

  get canRunApp() {
    return this.compiledOK;
  }

  onRunApp() {
    const code = encodeURIComponent(this.codeDefinitions);
    window.open(`http://armlite.metalup.org?submit=${code}`, '_blank');
  }

  onSubmit() {
    this.compiledOK = false;
    this.pendingSubmit = false;
    this.validationFail = this.rulesService.checkRules(Applicability.functions, this.codeDefinitions);
    if (!this.validationFail) {
      this.compileServer.submitCode(this.taskId, this.codeDefinitions).pipe(first()).subscribe(rr => {
        this.result = rr;
        this.compiledOK = !(this.result.cmpinfo || this.result.stderr) && this.result.outcome == 15;
        if (this.compiledOK) {
          this.compileServer.setUserDefinedCode(this.codeDefinitions);
          this.unsubmittedCode = "";
          // load code but don't update state
          this.taskService.loadCode(this.taskId, 1).then(c => this.setCodeVersion(c, false));
        }
      });
    }
  }

  loadServerCode(index: number) {
    this.taskService.loadCode(this.taskId, index).then(c => this.setCodeVersion(c, true));
  }

  canNewerCode() {
    return this.currentCodeVersion.Version > 1 || (this.currentCodeVersion.Version == 1 && !!this.unsubmittedCode.trim());
  }

  getUnsubmittedCodeVersion() {
    return {
      TaskId: 0,
      Code: this.unsubmittedCode,
      Version: 0,
      HasPreviousVersion: true
    } as ICodeUserView;
  }

  newerCode() {
    if (this.currentCodeVersion.Version == 1) {
      // go to last submitted code
      this.setCodeVersion(this.getUnsubmittedCodeVersion(), true);
    }
    else {
      this.loadServerCode(this.currentCodeVersion.Version - 1);
    }
  }

  canOlderCode() {
    return this.currentCodeVersion.HasPreviousVersion;
  }

  olderCode() {
    this.loadServerCode(this.currentCodeVersion.Version + 1);
  }

  private placeholderMap: Map<string, string> = new Map(
    []);

  get placeholder() {
    return this.placeholderMap.get(this.compileServer.selectedLanguage) || '';
  }

  get paneSize() {
    return "pane-size-large";
  }

  private sub?: Subscription;

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(t => {
      if (t.Id !== this.taskId) {
        this.taskId = t.Id;
        this.canPaste = t.PasteCode;
        this.taskService.loadCode(this.taskId, 0).then(c => {
          this.setCodeVersion(c, true);
          if (c.HasPreviousVersion) {
            this.loadServerCode(1);
          }
        });
      }
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }

  @ViewChild('codeArea', { static: true }) 
  taElement?: ElementRef;

  lineAndColumn = [0,0];

  refreshCursorPosition() {
    const ta = this.taElement?.nativeElement as any;
    const lineNo = ta.value.substr(0, ta.selectionStart).split(/\r?\n|\r/).length;
    const toSelection =  ta.value.substr(0, ta.selectionStart).split("").reverse().join("");
    const fromLineArray = toSelection.match(/.*$/m);
    const fromLine = fromLineArray[0];
    const col = fromLine.length;

    this.lineAndColumn = [lineNo, col];
  }
}
