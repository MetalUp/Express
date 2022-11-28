import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { TaskService } from '../services/task.service';


@Component({
  selector: 'app-task-view',
  templateUrl: './task-view.component.html',
  styleUrls: ['./task-view.component.css']
})
export class TaskViewComponent implements OnInit, OnDestroy {
  title = 'ile-client';

  constructor(private taskService: TaskService, private route: ActivatedRoute) {
  }

  language: string = '';

  private sub1?: Subscription;
  private sub2?: Subscription;

  ngOnInit(): void {
    this.sub1 = this.route.paramMap.subscribe(pm => {
      const id = pm.get('id') || "";
      if (id) {
        const taskId = parseInt(id);
        this.taskService.loadTask(taskId);
      }
    });

    this.sub2 = this.taskService.currentTask.subscribe(t => {
      this.language = t.Language;
    })
  }

  ngOnDestroy(): void {
    if (this.sub1) {
      this.sub1.unsubscribe();
    }
    if (this.sub2) {
      this.sub2.unsubscribe();
    }
  }
}
