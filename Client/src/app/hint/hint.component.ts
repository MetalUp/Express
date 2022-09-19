import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTask, ITask } from '../services/task';
import { TaskService } from '../services/task.service';
import { of, Subscription } from 'rxjs';
import { catchError, first } from 'rxjs/operators';

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

  hintIndex = -1;

  get title() {
    return `Hint: ${this.hintIndex >= 0 ? this.hintIndex + 1 : ''}`;
  }


  hasNextHint() {
    return this.hintIndex + 1 < this.currentTask.Hints.length;
  }

  hasPreviousHint() {
    return this.hintIndex - 1 >= 0;
  }

  handleError(e : unknown) {
    console.log("error getting hint ");
    return of('');
  }

  onHint() {
    const hintFileName = this.currentTask.Hints[this.hintIndex];

    if (hintFileName) {
      this.taskService.getHtml(hintFileName).pipe(first()).pipe(catchError(this.handleError)).subscribe(h => this.currentHint = h);
    }
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
      if (this.currentTask.Hints.length > 0) {
        this.currentHint = 'Click Next to use the first Hint';
      }
      else {
        this.currentHint = 'There are no Hints for this task.'
      }
      this.hintIndex = -1;
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}

