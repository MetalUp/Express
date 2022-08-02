import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EmptyTask, ITask } from './task';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  constructor(private http: HttpClient, private router: Router, private route: ActivatedRoute) {
    this.route.queryParams.subscribe(params => {
      const taskId = params['task'];
      if (taskId) {
        this.load(taskId);
      }
    });
   }

  get currentSubjectTask() {
    return this._currentSubjectTask;
  }

  get currentTask() {
    return this._currentTask;
  }

  private _currentSubjectTask = new Subject<ITask>() 
  private _currentTask : ITask = EmptyTask; 

  private updateLanguage(task: ITask, taskId: string) {
    const params = {
      "language": task.Language,
      "task": taskId
    }
    this.router.navigate(['/'], { queryParams: params });
  }

  load(taskId: string ) {
    const options = {
      withCredentials: true,
    }

    return this.http.get<ITask>(`content/${taskId}.json`, options).pipe(task => {
      task.subscribe(t => {
        this.updateLanguage(t, taskId);
        this._currentSubjectTask.next(t);
        this._currentTask = t;
      });
      return task;
    });
  }

  getHtml(task : ITask) {
    const options = {
      withCredentials: true,
      responseType: 'text' as const
    }

    return this.http.get(`content/${task.Description}`, options);  
  }
}
