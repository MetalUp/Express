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

  hintIndex = 0;

  hasHint() {
    return this.hintIndex < this.currentTask.Hints.length;
  }

  hasNextHint() {
    return this.hintIndex + 1 < this.currentTask.Hints.length;
  }

  hasPreviousHint() {
    return this.hintIndex - 1 >= 0;
  }

  onHint() {
    const hint = this.currentTask.Hints[this.hintIndex];

    if (hint) {
      this.taskService.getHtml(hint).subscribe(h => this.currentHint = h);
    }
  }

  onFirstHint() {
    this.hintIndex = 0;
    return this.onHint();
  }

  onPreviousHint() {
    this.hintIndex--;
    return this.onHint();
  }

  onNextHint() {
    this.hintIndex++;
    return this.onHint();
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
