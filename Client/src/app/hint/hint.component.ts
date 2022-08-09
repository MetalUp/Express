import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTask, ITask } from '../services/task';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-hint',
  templateUrl: './hint.component.html',
  styleUrls: ['./hint.component.css']
})
export class HintComponent implements OnInit, OnDestroy {

  currentTask: ITask = EmptyTask;

  get hintHtml() {
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
    const hintFileName = this.currentTask.Hints[this.hintIndex];

    if (hintFileName) {
      this.taskService.getHtml(hintFileName).pipe(first()).subscribe(h => this.currentHint = h);
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
      this.currentHint = '';
      this.hintIndex = 0;
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}

