import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EmptyTask, ITask } from '../services/task';
import { TaskService } from '../services/task.service';

@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.css']
})
export class TaskComponent implements OnInit {

  currentTask: ITask = EmptyTask;

  innerHtml = '';

  constructor(private taskService: TaskService) { }

  ngOnInit(): void {

    this.taskService.currentSubjectTask.subscribe(task => {
      this.currentTask = task;
      this.taskService.getHtml(this.currentTask).subscribe(h => this.innerHtml = h);
    })
  }
}
