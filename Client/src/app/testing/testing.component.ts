import { Component, OnDestroy, OnInit } from '@angular/core';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { JobeServerService } from '../services/jobe-server.service';
import { EmptyRunResult, RunResult } from '../services/run-result';
import { wrapTests } from '../languages/language-helpers';

@Component({
  selector: 'app-testing',
  templateUrl: './testing.component.html',
  styleUrls: ['./testing.component.css']
})
export class TestingComponent implements OnInit, OnDestroy {

  constructor(
    private jobeServer: JobeServerService,
    private taskService: TaskService
  ) { }

  hasTests() {
    return !!this.tests;
  }

  message() {
    if (this.result.outcome === 0 || !this.canRunTests()){
      this.result = EmptyRunResult;
      if (this.canRunTests()) {
        return 'Tests not yet run on current function definition.';
      }
      return 'This task defines automated tests. These may only be run once Function definition code has been submitted and successfully compiles.';
    }
    return '';
  }

  stdout() {
    if (this.result.outcome !== 0){
      return `stdout: ${this.result.stdout}`;
    }
    return '';
  }

  cmpinfo() {
    if (this.result.outcome !== 0){
      return `cmpinfo: ${this.result.cmpinfo}`;
    }
    return '';
  }

  stderr() {
    if (this.result.outcome !== 0){
      return `stderr: ${this.result.stderr} `;
    }
    return '';
  }

  tests = ''

  canRunTests() {
    return this.jobeServer.hasFunctionDefinitions();
  }

  submitting = false;
  result: RunResult = EmptyRunResult;
  testedOk = false;

  onRunTests() {
    this.submitting = true;
    const code = wrapTests(this.jobeServer.selectedLanguage, this.tests);
    this.jobeServer.submit_run(code).pipe(first()).subscribe(rr => {
      this.result = rr;
      this.testedOk = !(this.result.cmpinfo || this.result.stderr) && this.result.outcome == 15;
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
