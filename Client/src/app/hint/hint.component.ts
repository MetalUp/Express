import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTaskUserView, ITaskUserView } from '../models/task-user-view';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { EmptyHintUserView, IHintUserView } from '../models/hint-user-view';
import { nextHintEnabledTooltip, nextHintDisabledTooltip, previousHintEnabledTooltip, previousHintDisabledTooltip, newHintDisabledTooltip, newHintEnabledTooltip } from '../constants/tooltips';


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

    if (this.canGetNextHint()) {
      return "Click 'Next Hint' to use the first Hint";
    }

    if (this.canViewNextHint()) {
      return "Click '>' to see your first used hint";
    }

    return 'There are no Hints for this task.'
  }

  private sub?: Subscription;

  get title() {
    return this.currentHint.Title || "Hint";
  }

  get nextHintTooltip() {
    return this.canViewNextHint() ? nextHintEnabledTooltip : nextHintDisabledTooltip;
  }

  get previousHintTooltip() {
    return this.canViewPreviousHint() ? previousHintEnabledTooltip : previousHintDisabledTooltip;
  }

  get newHintTooltip() {
    return this.canGetNextHint() ? newHintEnabledTooltip : newHintDisabledTooltip;
  }

  canGetNextHint() {
    return !!this.currentHint.NextHintNo && !this.currentHint.NextHintAlreadyUsed;
  }

  canViewNextHint() {
    return !!this.currentHint.NextHintNo && this.currentHint.NextHintAlreadyUsed;
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
      this.currentHint = h;
      if (newHint) {
        this.taskService.loadTask(this.currentTask.Id);
      }
    });
  }

  get paneSize() {
    return "pane-size-medium";
  }

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(task => {
      const currentHintNo = this.currentHint.HintNo;
      const newTask = this.currentTask.Id !== task.Id;
      this.currentTask = task;
      this.getHint(newTask ? 0 : currentHintNo, false);
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}