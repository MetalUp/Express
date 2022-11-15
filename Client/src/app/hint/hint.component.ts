import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTaskUserView, ITaskUserView } from '../models/task';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'app-hint',
  templateUrl: './hint.component.html',
  styleUrls: ['./hint.component.css']
})
export class HintComponent implements OnInit, OnDestroy {

  currentTask: ITaskUserView = EmptyTaskUserView;
 
  get hintHtml() {
    if (this.currentTask.CurrentHintContent){
      return this.currentTask.CurrentHintContent;
    }

    if (this.currentTask.CurrentHintNo === 0) {
      return 'Click Next to use the first Hint';
    }
   
    return 'There are no Hints for this task.'
  }

  constructor(private taskService: TaskService) { }

  private sub?: Subscription;

  get title() {
    return this.currentTask.CurrentHintTitle;
  }

  canGetNextHint() {
    return !!this.currentTask.NextHintNo;
  }

  canViewNextHint() {
    return !!this.currentTask.NextHintNo;
  }

  canViewPreviousHint() {
    return !!this.currentTask.PreviousHintNo;
  }

  handleError(e : unknown) {
    console.log("error getting hint ");
    //this.message = 'error getting hint';
  }

  viewPreviousHint() {
    this.taskService.loadTask(this.currentTask.Id, this.currentTask.PreviousHintNo!);
  }

  viewNextHint() {
    this.taskService.loadTask(this.currentTask.Id, this.currentTask.NextHintNo!);
  }

  getNextHint() {
    this.taskService.loadTask(this.currentTask.Id, this.currentTask.NextHintNo!);
  }

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(task => {
      this.currentTask = task;
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}

