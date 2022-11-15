import { Component, OnDestroy, OnInit } from '@angular/core';
import {  EmptyTaskUserView, ITaskUserView } from '../models/task';
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
    return this.currentTask.IsCompleted && this.currentTask.NextTaskEnabled  && !!this.currentTask.NextTaskId;
  }

  viewNextTask() {
    return this.taskService.gotoTask(this.currentTask.NextTaskId!);
  }

  canReturnToAssignment() {
    return this.currentTask.IsCompleted && !this.currentTask.NextTaskId;
  }

  returnToAssignment() {
    // todo
  }


  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(task => {
      this.currentTask = task;
      this.taskHtml = this.currentTask.Description;
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
