import { Component, OnDestroy, OnInit } from '@angular/core';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { JobeServerService } from '../services/jobe-server.service';
import { EmptyRunResult, getResultOutcome, RunResult } from '../services/run-result';
import { wrapTests } from '../language-helpers/language-helpers';
import { RulesService } from '../services/rules.service';
import { ErrorType } from '../services/rules';

@Component({
  selector: 'app-testing',
  templateUrl: './testing.component.html',
  styleUrls: ['./testing.component.css']
})
export class TestingComponent implements OnInit, OnDestroy {

  constructor(
    private jobeServer: JobeServerService,
    private taskService: TaskService,
    private rulesService: RulesService
  ) { }

  hasTests() {
    return !!this.tests;
  }

  message() {
    if (this.canRunTests() && this.result.outcome === 0) {
      return 'Tests not yet run on current function definition.';
    }
    if (this.hasTests() && this.result.outcome === 0) {
      return 'This task defines automated tests, which may be run once Function definition code has successfully compiled.';
    }
    if (!this.hasTests()){
      return 'There are no Tests defined for this task'; 
    }
    return this.currentResultMessage;
  }

  tests = ''

  canRunTests() {
    if (!this.hasTests()){
      return false;
    }

    if (this.jobeServer.hasFunctionDefinitions()) {
      return this.result.outcome === 0;
    }
    this.testedOk = false;
    this.result = EmptyRunResult;
    return false;
  }

  submitting = false;
  result: RunResult = EmptyRunResult;
  testedOk = false;
  currentResultMessage = '';

  private handleResult(result: RunResult) {
    this.result = result;
    this.testedOk = !(result.cmpinfo || result.stderr) && result.outcome == 15;

    if (this.testedOk) {
      // all OK
      this.currentResultMessage = result.stdout;
    }
    else if (result.stdout && result.stderr) {
      // expected test fail
      this.currentResultMessage = result.stdout;
    }
    else if (result.stderr) {
      // unexpected runtime error
      this.currentResultMessage = this.rulesService.filter(this.jobeServer.selectedLanguage, ErrorType.stderr, result.stderr);
    }
    else if (result.cmpinfo) {
      // compile error
      this.currentResultMessage = "Your function signature does not match that expected by the tests. Re-read the Task and, if you can't see why your function signature is wrong, use a Hint. " + result.cmpinfo;
    }
    else {
      this.currentResultMessage = getResultOutcome(result.outcome);
    }
  }

  onRunTests() {
    this.submitting = true;
    const code = wrapTests(this.jobeServer.selectedLanguage, this.tests);
    this.jobeServer.submit_run(code).pipe(first()).subscribe(rr => {
      this.handleResult(rr);
      this.submitting = false;
    });
  }

  private sub?: Subscription;

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(task => {
      if (task.Tests) {
        this.taskService.getHtml(task.Tests).pipe(first()).subscribe(h => this.tests = h);
      }
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }

}
