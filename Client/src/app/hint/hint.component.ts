import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTaskUserView, ITaskUserView } from '../models/task';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { EmptyHintUserView, IHintUserView } from '../models/hint';


@Component({
  selector: 'app-hint',
  templateUrl: './hint.component.html',
  styleUrls: ['./hint.component.css']
})
export class HintComponent implements OnInit, OnDestroy {

  currentTask: ITaskUserView = EmptyTaskUserView;
  currentHint: IHintUserView = EmptyHintUserView;

  get hintHtml() {
    if (this.currentHint.Contents) {
      return this.currentHint.Contents;
    }

    if (this.currentHint.NextHintNo && this.currentHint.CostOfNextHint > 0) {
      return "Click 'Next Hint' to use the first Hint";
    }

    return 'There are no Hints for this task.'
  }

  constructor(private taskService: TaskService) { }

  private sub?: Subscription;

  get title() {
    return this.currentHint.Title || "Hint";
  }

  canGetNextHint() {
    return !!this.currentHint.NextHintNo && this.currentHint.CostOfNextHint > 0;
  }

  canViewNextHint() {
    return !!this.currentHint.NextHintNo && this.currentHint.CostOfNextHint === 0;
  }

  canViewPreviousHint() {
    return !!this.currentHint.PreviousHintNo;
  }

  handleError(e: unknown) {
    console.log("error getting hint ");
    //this.message = 'error getting hint';
  }

  viewPreviousHint() {
    this.getHint(this.currentHint.PreviousHintNo!);
  }

  viewNextHint() {
    this.getHint(this.currentHint.NextHintNo!);
  }

  getNextHint() {
    this.getHint(this.currentHint.NextHintNo!);
  }

  getHint(hintNo: number) {
    this.taskService.loadHint(this.currentTask.Id, hintNo).then(h => {
      if (h.CostOfNextHint >= 0){
        this.currentHint = h;
      }
      else {
        console.log(`error getting hint ${hintNo} for task ${this.currentTask.Id}`);
      }
    });
  }

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(task => {
      this.currentTask = task;
      this.getHint(0);
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}