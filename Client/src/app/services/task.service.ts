import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { EmptyTask, ITask, Task } from './task';
import { from, Subject } from 'rxjs';
import { ConfigService, ErrorWrapper, RepLoaderService } from '@nakedobjects/services';
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

  private getChoicesValue(member: Ro.PropertyMember) {
    const raw = member.value().scalar() as number;
    const choices = member.choices();
    for (const k in choices) {
      const v = choices[k];
      if (v.scalar() === raw) {
        return k;
      }
    }
    return undefined;
  }

  private setValue(task: any, member: Ro.PropertyMember) {
    const attachmentLink = member.attachmentLink();
    if (attachmentLink) {
      const href = attachmentLink.href();
      const mt = attachmentLink.type().asString;
      task[member.id()] = [href, mt];
    }
    else if (member.entryType() == EntryType.Choices) {
      task[member.id()] = this.getChoicesValue(member);
    }
    else if (member.isScalar()) {
      task[member.id()] = member.value().scalar();
    }
    else {
      task[member.id()] = member.value().getHref();
    }
  }

  private convertToTask(rep: DomainObjectRepresentation) {
    if (rep && Object.keys(rep.propertyMembers()).length > 0) {
      const task = new Task() as any;
      const members = rep.propertyMembers()
      for (const k in members) {
        const member = members[k];
        this.setValue(task, member);
      }
      return task as ITask;
    }
    return EmptyTask;
  }

  loadTask(taskId: string) {
    const object = new DomainObjectRepresentation();
    object.hateoasUrl = `${this.configService.config.appPath}/objects/Model.Types.Task/${taskId}`;

    this.repLoader.populate<Ro.DomainObjectRepresentation>(object, true)
      .then((obj: Ro.DomainObjectRepresentation) => {
        const task = this.convertToTask(obj);
        this.currentTaskAsSubject.next(task);
      })
      .catch((e : ErrorWrapper) => {
        console.log(`${e.title}:${e.description}`);
        this.currentTaskAsSubject.next(EmptyTask);
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

  getFile(urlAndMediaType: [string, string]) {
    return  from(this.repLoader.getFile(urlAndMediaType[0], urlAndMediaType[1], true));
  }
}
