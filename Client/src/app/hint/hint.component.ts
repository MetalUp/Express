import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTask, ITask } from '../models/task';
import { TaskService } from '../services/task.service';
import { of, Subscription } from 'rxjs';
import { catchError, first } from 'rxjs/operators';
import { EmptyHint, IHint } from '../models/hint';

@Component({
  selector: 'app-hint',
  templateUrl: './hint.component.html',
  styleUrls: ['./hint.component.css']
})
export class HintComponent implements OnInit, OnDestroy {

  currentTask: ITask = EmptyTask;
  currentHint: IHint = EmptyHint;
  message: string = '';

  get hintHtml() {
    if (this.currentHint.HtmlContent){
      return this.currentHint.HtmlContent;
    }

    if (this.message) {
      return this.message;
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
    this.message = 'error getting hint';
  }

  onHint() {
    this.currentHint = this.currentTask.Hints[this.hintIndex];

    if (this.currentHint.HtmlFile[0] && !this.currentHint.HtmlContent) {
      this.message = "";
      this.taskService.getFile(this.currentHint.HtmlFile)
        .then(h => this.currentHint.HtmlContent = h)
        .catch(e => this.handleError(e));
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

