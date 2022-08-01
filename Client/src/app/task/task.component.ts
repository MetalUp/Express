import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TaskService } from '../services/task.service';

@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.css']
})
export class TaskComponent implements OnInit {

  taskId: string = ''

  constructor(private route: ActivatedRoute, private task: TaskService) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.taskId = params['task'];
      if (this.taskId) {
        this.task.load(this.taskId);
      }
    });
  }
}
