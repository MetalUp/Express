import { Component, OnDestroy, OnInit } from '@angular/core';
import { EmptyTask, ITask } from '../services/task';
import { TaskService } from '../services/task.service';
import { of, Subscription } from 'rxjs';
import { catchError, first } from 'rxjs/operators';

@Component({
  selector: 'app-task-description',
  templateUrl: './task-description.component.html',
  styleUrls: ['./task-description.component.css']
})
export class TaskDescriptionComponent implements OnInit, OnDestroy {

  currentTask: ITask = EmptyTask;

  taskHtml = '';

  constructor(private taskService: TaskService) { }

  private sub?: Subscription;

  hasPreviousTask() {
    return !!this.currentTask.PreviousTask;
  }

  onPreviousTask() {
    this.taskService.gotoTask(this.currentTask.PreviousTask!);
  }

  hasNextTask() {
    return !!this.currentTask.NextTask;
  }

  onNextTask() {
    this.taskService.gotoTask(this.currentTask.NextTask!);
  }

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(task => {
      this.currentTask = task;
      this.taskService.getFile(this.currentTask.Description)
        .then(h => this.taskHtml = h)
        .catch(_ => this.taskHtml = '');
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
