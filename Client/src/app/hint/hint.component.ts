import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTask, ITask } from '../services/task';
import { TaskService } from '../services/task.service';
import { of, Subscription } from 'rxjs';
import { catchError, first } from 'rxjs/operators';
import { EmptyHint, IHint } from '../services/hint';

@Component({
  selector: 'app-hint',
  templateUrl: './hint.component.html',
  styleUrls: ['./hint.component.css']
})
export class HintComponent implements OnInit, OnDestroy {

  currentTask: ITask = EmptyTask;
  currentHint: IHint = EmptyHint;
  currentHtml: string = '';

  get hintHtml() {
    if (this.currentHtml){
      return this.currentHtml;
    }

    if (this.currentTask.Hints.length > 0) {
      return 'Click Next to use the first Hint';
    }
   
    return 'There are no Hints for this task.'
  }

  constructor(private taskService: TaskService) { }

  private sub?: Subscription;

  hintIndex = -1;

  get title() {
    return `Hint: ${this.currentHint.Title}`;
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
    this.currentHint = this.currentTask.Hints[this.hintIndex];

    if (this.currentHint.HtmlFile[0]) {
      this.taskService.getFile(this.currentHint.HtmlFile)
        .then(h => this.currentHtml = h)
        .catch(_ => this.currentHtml = '');
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
      this.hintIndex = -1;
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}

