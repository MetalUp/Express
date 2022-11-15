import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTask, EmptyTaskUserView, ITask, ITaskUserView } from '../models/task';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { EmptyHint, IHint } from '../models/hint';

@Component({
  selector: 'app-hint',
  templateUrl: './hint.component.html',
  styleUrls: ['./hint.component.css']
})
export class HintComponent implements OnInit, OnDestroy {

  currentTask: ITaskUserView = EmptyTaskUserView;
  currentHint: IHint = EmptyHint;
  message: string = '';

  get hintHtml() {
    if (this.currentTask.CurrentHintContent){
      return this.currentTask.CurrentHintContent;
    }

    if (this.message) {
      return this.message;
    }

    if (this.currentTask.CurrentHintNo === 0) {
      return 'Click Next to use the first Hint';
    }
   
    return 'There are no Hints for this task.'
  }

  constructor(private taskService: TaskService) { }

  private sub?: Subscription;

  hintIndex = -1;

  get marks() {
    return this.currentHint.CostInMarks === 1 ? "mark" : "marks";
  }

  get title() {
    return this.currentTask.CurrentHintTitle;
  }

  hasNextHint() {
    return !!this.currentTask.NextHintNo;
  }

  hasPreviousHint() {
    return !!this.currentTask.PreviousHintNo;
  }

  handleError(e : unknown) {
    console.log("error getting hint ");
    this.message = 'error getting hint';
  }

  onHint() {
    // this.currentHint = this.currentTask.Hints[this.hintIndex];

    // if (this.currentHint.HtmlFile[0] && !this.currentHint.HtmlContent) {
    //   this.message = "";
    //   this.taskService.getFile(this.currentHint.HtmlFile)
    //     .then(h => this.currentHint.HtmlContent = h)
    //     .catch(e => this.handleError(e));
    // }
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
      this.currentHint = EmptyHint;
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}

