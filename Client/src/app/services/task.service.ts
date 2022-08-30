import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ITask } from './task';
import { Subject } from 'rxjs';
import { first } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  constructor(private http: HttpClient, private router: Router, route: ActivatedRoute) {
    route.queryParams.subscribe(params => {
      const taskId = params['task'];
      if (taskId) {
        this.load(taskId);
      }
    });
  }

  get currentTask() {
    return this.currentTaskAsSubject;
  }

  private currentTaskAsSubject = new Subject<ITask>()

  private setDefaultTask() {
    const params = {
      "task": 'default_python'
    }
    this.router.navigate(['/'], { queryParams: params });
  }

  private setTask(taskId : string) {
    const params = {
      "task": taskId
    }
    this.router.navigate(['/'], { queryParams: params });
  }

  private load(taskId: string) {
    const options = {
      withCredentials: true,
    }

    this.http.get<ITask>(`content/${taskId}.json`, options).pipe(first()).subscribe(t => {
      this.currentTaskAsSubject.next(t);
    });
  }

  gotoTask(taskId: string){
    if (taskId.endsWith(".json")){
      taskId = taskId.replace(".json", "");
    }

    this.setTask(taskId);
  }

  getHtml(fileName: string) {
    const options = {
      withCredentials: true,
      responseType: 'text' as const
    }

    return this.http.get(`content/${fileName}`, options);
  }
}
