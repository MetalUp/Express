import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

export interface ITask {
  language: string;
}


@Injectable({
  providedIn: 'root'
})
export class TaskService {

  task: ITask = { language: '' }

  constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router) { }

  load(taskId: string) {

    const options = {
      withCredentials: true,
    }

    this.http.get<ITask>(`content/${taskId}.json`, options).subscribe(task => {
        this.task = task;
        const url = this.router.createUrlTree([{"language": task.language}]);
        this.router.navigateByUrl(url);
      }
    );
  }
}
