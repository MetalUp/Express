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

  constructor(private taskService: TaskService) { }

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

  viewPreviousHint() {
    this.getHint(this.currentHint.PreviousHintNo!, false);
  }

  viewNextHint() {
    this.getHint(this.currentHint.NextHintNo!, false);
  }

  getNextHint() {
    this.getHint(this.currentHint.NextHintNo!, true);
  }

  getHint(hintNo: number, newHint: boolean) {
    
    this.taskService.loadHint(this.currentTask.Id, hintNo).then(h => {
      if (h.CostOfNextHint >= 0){
        this.currentHint = h;
        if (newHint) {
          this.taskService.loadTask(this.currentTask.Id);
        }
      }
      else {
        console.log(`error getting hint ${hintNo} for task ${this.currentTask.Id}`);
      }
    });
  }

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(task => {
      const newTask = this.currentTask.Id !== task.Id; 
      this.currentTask = task;
      if (newTask){
        this.getHint(0, false);
      }
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}