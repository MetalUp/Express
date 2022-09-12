import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { TaskService } from '../services/task.service';


@Component({
  selector: 'app-student-view',
  templateUrl: './student-view.component.html',
  styleUrls: ['./student-view.component.css']
})
export class StudentViewComponent implements OnInit, OnDestroy {
  title = 'ile-client';

  constructor(private taskService: TaskService, private route: ActivatedRoute) {
  }

  language: string = '';

  private sub1?: Subscription;
  private sub2?: Subscription;

  ngOnInit(): void {
    this.sub1 = this.route.paramMap.subscribe(pm => {
      const taskId = pm.get('id');
      if (taskId) {
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
