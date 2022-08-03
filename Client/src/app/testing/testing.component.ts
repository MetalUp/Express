import { Component, OnDestroy, OnInit } from '@angular/core';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { JobeServerService } from '../services/jobe-server.service';

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

  message = 'This task defines automated tests. These may only be run once Function definition code has been submitted and successfully compiles.';

  tests = ''

  canRunTests() {
    return this.jobeServer.hasFunctionDefinitions();
  }

  onRunTests() {

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
