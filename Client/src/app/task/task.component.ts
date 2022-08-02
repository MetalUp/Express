import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTask, ITask } from '../services/task';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.css']
})
export class TaskComponent implements OnInit, OnDestroy {

  currentTask: ITask = EmptyTask;

  innerHtml = '';

  get innerHintHtml() {
    return this.currentHint;
  }

  private currentHint = '';

  constructor(private taskService: TaskService) { }

  private sub?: Subscription;

  onHint() {
    const hint = this.currentTask.Hints[0];

    if (hint) {
      this.taskService.getHtml(hint).subscribe(h => this.currentHint = h);
    }
  }


  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(task => {
      this.currentTask = task;
      this.taskService.getHtml(this.currentTask.Description).subscribe(h => this.innerHtml = h);
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
