import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { EmptyTask, ITask, Task } from './task';
import { of, Subject } from 'rxjs';
import { first } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ConfigService, RepLoaderService } from '@nakedobjects/services';
import * as Ro from '@nakedobjects/restful-objects';
import { DomainObjectRepresentation, EntryType, Value } from '@nakedobjects/restful-objects';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  constructor(private http: HttpClient,
              private router: Router,
              private configService : ConfigService,
              private repLoader: RepLoaderService) {
  }

  get currentTask() {
    return this.currentTaskAsSubject;
  }

  private currentTaskAsSubject = new Subject<ITask>()

  loadTaskOld(taskId: string) {
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

  private setValue(task: any, member: Ro.PropertyMember) {
    if (member.entryType() == EntryType.Choices) {
      const raw = member.value().scalar() as number;
      const choices = member.choices();
      for (const k in choices) {
        const v = choices[k];
        if (v.scalar() === raw) {
          task[member.id()] = k.replace(' ', '');
        }
      }
    }
    else if (member.isScalar()) {
      task[member.id()] = member.value().scalar();
    }
    else {
      task[member.id()] = member.value().getHref();
    }
  }

  private convertToTask(rep: DomainObjectRepresentation) {
    const task = new Task() as any;
    const members = rep.propertyMembers()
    for (const k in members) {
      const member = members[k];
      this.setValue(task, member);
    }
    return task as ITask;
  }

  loadTask(taskId: string) {

    const object = new DomainObjectRepresentation();
    object.hateoasUrl = `${this.configService.config.appPath}/objects/Model.Types.Task/${taskId}`;

    this.repLoader.populate<Ro.DomainObjectRepresentation>(object, true)
      .then((obj: Ro.DomainObjectRepresentation) => {
        const task = this.convertToTask(obj);
        this.currentTaskAsSubject.next(task);
      })
      .catch((e) => {
        throw e;
      });
  }

  gotoTask(taskId: string) {
    var segments = taskId.split('/');
    var key = segments[segments.length -1];

    this.router.navigate([`/task/${key}`]);
  }

  getHtml(fileName: string) {
    const options = {
      withCredentials: true,
      responseType: 'text' as const
    }

    return this.http.get(`content/${fileName}`, options);
  }
}
