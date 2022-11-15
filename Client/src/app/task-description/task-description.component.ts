import { Component, OnDestroy, OnInit } from '@angular/core';
import {  EmptyTaskUserView, ITaskUserView } from '../models/task';
import { TaskService } from '../services/task.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-task-description',
  templateUrl: './task-description.component.html',
  styleUrls: ['./task-description.component.css']
})
export class TaskDescriptionComponent implements OnInit, OnDestroy {

  currentTask: ITaskUserView = EmptyTaskUserView;

  taskHtml = '';

  constructor(private taskService: TaskService) { }

  private sub?: Subscription;

  hasPreviousTask() {
    return !!this.currentTask.PreviousTaskId;
  }

  onPreviousTask() {
    this.taskService.gotoTask(this.currentTask.Id!, 0);
  }

  hasNextTask() {
    return !!this.currentTask.NextTaskId;
  }

  onNextTask() {
    this.taskService.gotoTask(this.currentTask.NextTaskId!, 0);
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
