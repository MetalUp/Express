import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ITask } from './task';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  constructor(private http: HttpClient, private router: Router) { }

  private updateLanguage(task: ITask, taskId: string) {
    const params = {
      "language": task.language,
      "task": taskId
    }
    this.router.navigate(['/'], { queryParams: params });
  }

  load(taskId: string) {

    const options = {
      withCredentials: true,
    }

    return this.http.get<ITask>(`content/${taskId}.json`, options).pipe(task => {
      task.subscribe(t => this.updateLanguage(t, taskId));
      return task;
    });
  }

  getHtml(task : ITask) {
    const options = {
      withCredentials: true,
      responseType: 'text' as const
    }

    return this.http.get(`content/${task.description}`, options);  
  }
}
