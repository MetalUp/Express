import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTaskUserView, ITaskUserView } from '../models/task-user-view';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

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

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(task => {
      const newTask = this.currentTask.Id !== task.Id;
      this.currentTask = task;
      if (newTask) {
        this.taskHtml = this.currentTask.Description;
      }
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
