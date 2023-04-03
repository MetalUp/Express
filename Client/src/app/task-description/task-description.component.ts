import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTaskUserView, ITaskUserView } from '../models/task-user-view';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { newTaskDisabledTooltip, newTaskEnabledTooltip, nextTaskDisabledTooltip, nextTaskEnabledTooltip, previousTaskDisabledTooltip, previousTaskEnabledTooltip, returnToAssignmentDisabledTooltip, returnToAssignmentEnabledTooltip } from '../constants/tooltips';

@Component({
  selector: 'app-task-description',
  templateUrl: './task-description.component.html',
  styleUrls: ['./task-description.component.css']
})
export class TaskDescriptionComponent implements OnInit, OnDestroy {

  currentTask: ITaskUserView = EmptyTaskUserView;

  taskHtml = '';

  constructor(private taskService: TaskService, private router: Router) { }

  private sub?: Subscription;

  get nextTaskTooltip() {
    return  this.canViewNextTask() ?  nextTaskEnabledTooltip : nextTaskDisabledTooltip;
  }

  get previousTaskTooltip() {
    return  this.canViewPreviousTask() ?  previousTaskEnabledTooltip : previousTaskDisabledTooltip;
  }

  get newTaskTooltip() {
    return  this.canGetNextTask() ?  newTaskEnabledTooltip : newTaskDisabledTooltip;
  }

  get returnToAssignmentTooltip() {
    return  this.canReturnToAssignment() ?  returnToAssignmentEnabledTooltip : returnToAssignmentDisabledTooltip;
  }

  canViewPreviousTask() {
    return !!this.currentTask.PreviousTaskId;
  }

  viewPreviousTask() {
    this.taskService.gotoTask(this.currentTask.PreviousTaskId!);
  }

  canViewNextTask() {
    return !!this.currentTask.NextTaskId && this.currentTask.NextTaskIsStarted;
  }

  viewNextTask() {
    return this.taskService.gotoTask(this.currentTask.NextTaskId!);
  }

  canGetNextTask() {
    return this.currentTask.Completed && !!this.currentTask.NextTaskId && !this.currentTask.NextTaskIsStarted;
  }

  getNextTask() {
    return this.taskService.gotoTask(this.currentTask.NextTaskId!);
  }

  canReturnToAssignment() {
    return this.currentTask.Completed && !this.currentTask.NextTaskId;
  }

  returnToAssignment() {
    const asn = `Model.Types.Assignment--${this.currentTask.AssignmentId}`;
    const tree = this.router.createUrlTree(['/dashboard/object'], { queryParams: { "o1": asn } });
    this.router.navigateByUrl(tree);
  }

  get paneSize() {
    return "pane-size-large";
  }

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(task => {
      const newTask = this.currentTask.Id !== task.Id;
      this.currentTask = task;
      if (newTask) {
        this.taskHtml = this.currentTask.Description;
      }
    });
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
