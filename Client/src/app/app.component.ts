import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { TaskService } from './services/task.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'ile-client';

  constructor(private taskService: TaskService) { }

  language: string = '';

  private sub?: Subscription;

  ngOnInit(): void {
    this.sub = this.taskService.currentTask.subscribe(t => {
      this.language = t.Language;
    })
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
