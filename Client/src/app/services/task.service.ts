import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EmptyTask, ITask } from './task';
import { of, Subject } from 'rxjs';
import { first } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  constructor(private http: HttpClient, private router: Router) {
  }

  get currentTask() {
    return this.currentTaskAsSubject;
  }

  private currentTaskAsSubject = new Subject<ITask>()

  private setTask(taskId: string) {
    this.router.navigate([`/task/${taskId}`]);
  }

  loadTask(taskId: string) {
    const options = {
      withCredentials: true,
    }

    this.http.get<ITask>(`content/${taskId}.json`, options).pipe(first()).pipe(catchError(e => of(EmptyTask))).subscribe(t => {
      if (t.Language === '') {
        // empty task
        this.router.navigate(['/']);
      }
      else {
        this.currentTaskAsSubject.next(t);
      }
    });
  }

  gotoTask(taskId: string) {
    if (taskId.endsWith(".json")) {
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
