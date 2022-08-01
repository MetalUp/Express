import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { ITask, TaskService } from '../services/task.service';

@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.css']
})
export class TaskComponent implements OnInit {

  taskId: string = ''

  currentTask: ITask = { language: '', title: '', taskHtmlFile: '' };

  innerHtml = '';

  constructor(private route: ActivatedRoute, private taskService: TaskService) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.taskId = params['task'];
      if (this.taskId) {
        this.taskService.load(this.taskId).subscribe(task => {
          this.currentTask = task;
          this.taskService.getHtml(this.currentTask).subscribe(h => this.innerHtml = h);
        })
      }
    });
  }
}
