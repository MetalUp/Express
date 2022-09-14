import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ITask, Task } from './task';
import { Subject } from 'rxjs';
import { ConfigService, RepLoaderService } from '@nakedobjects/services';
import * as Ro from '@nakedobjects/restful-objects';
import { DomainObjectRepresentation, EntryType } from '@nakedobjects/restful-objects';

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

  private setValue(task: any, member: Ro.PropertyMember) {
    if (member.entryType() == EntryType.Choices) {
      const raw = member.value().scalar() as number;
      const choices = member.choices();
      for (const k in choices) {
        const v = choices[k];
        if (v.scalar() === raw) {
          task[member.id()] = k.replace(' ', ''); // to fix 'C Sharp'
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
        // todo
        throw e;
      });
  }

  gotoTask(taskUrl: string) {
    var segments = taskUrl.split('/');
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
