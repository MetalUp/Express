import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { EmptyTaskUserView, ITaskUserView, TaskUserView } from '../models/task';
import { EmptyHintUserView, HintUserView, IHintUserView } from '../models/hint';
import { Subject } from 'rxjs';
import { ContextService, ErrorWrapper, RepLoaderService } from '@nakedobjects/services';
import { ActionResultRepresentation, DomainObjectRepresentation, DomainServicesRepresentation, EntryType, IHateoasModel, InvokableActionMember, PropertyMember, Value } from '@nakedobjects/restful-objects';
import { Dictionary } from 'lodash';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  
  constructor(private router: Router,
    private contextService: ContextService,
    private repLoader: RepLoaderService) { 
  }

  getService() {
    if (this.taskAccess){
      return Promise.resolve(this.taskAccess);
    }

    return this.contextService.getServices()
      .then((services: DomainServicesRepresentation) => {
        const service = services.getService("Model.Functions.Services.TaskAccess");
        return this.repLoader.populate(service);
      })
      .then((service: IHateoasModel) => {
        this.taskAccess = service as DomainObjectRepresentation;
        return this.taskAccess;
      })
  }

  taskAccess?: DomainObjectRepresentation;

  get currentTask() {
    return this.currentTaskAsSubject;
  }

  private currentTaskAsSubject = new Subject<ITaskUserView>()

  private getChoicesValue(member: PropertyMember) {
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

  private setPropertyValue(toObj: any, member: PropertyMember) {
    const attachmentLink = member.attachmentLink();
    if (attachmentLink) {
      const href = attachmentLink.href();
      const mt = attachmentLink.type().asString;
      toObj[member.id()] = [href, mt];
    }
    else if (member.entryType() == EntryType.Choices) {
      toObj[member.id()] = this.getChoicesValue(member);
    }
    else if (member.isScalar()) {
      toObj[member.id()] = member.value().scalar();
    }
    else {
      toObj[member.id()] = member.value().getHref();
    }
  }

  private convertTo<T extends ITaskUserView | IHintUserView>(toObj: TaskUserView | HintUserView, rep: DomainObjectRepresentation) {
    if (rep && Object.keys(rep.propertyMembers()).length > 0) {
      const pMembers = rep.propertyMembers()
      for (const k in pMembers) {
        const member = pMembers[k];
        this.setPropertyValue(toObj, member);
      }
    }
    return toObj as T;
  }

  private convertToTask(rep: DomainObjectRepresentation, id: number) {
    return this.convertTo<ITaskUserView>(new TaskUserView(id), rep);
  }

  private convertToHint(rep: DomainObjectRepresentation) {
    return this.convertTo<IHintUserView>(new HintUserView(), rep);
  }

  params(taskId: number, currentHintNo?: number) {
    const params = { "taskId": new Value(taskId) } as Dictionary<Value>;

    if (currentHintNo != undefined) {
      params['hintNumber'] = new Value(currentHintNo);
    }

    return params;
  }

  loadTask(taskId: number) {

    this.getService().then(s => {
      const action = s.actionMember("GetTask") as InvokableActionMember;

      this.repLoader.invoke(action, this.params(taskId), {} as Dictionary<Object>)
        .then((ar: ActionResultRepresentation) => {
          var obj = ar.result().object()!;
          const task = this.convertToTask(obj, taskId);
          this.currentTaskAsSubject.next(task);
        })
        .catch((e: ErrorWrapper) => {
          console.log(`${e.title}:${e.description}`);
          this.currentTaskAsSubject.next(EmptyTaskUserView);
        });
    });
  }

  loadHint(taskId: number, hintId: number) {

    return this.getService().then(s => {
      const action = s.actionMember("GetHint") as InvokableActionMember;

      return this.repLoader.invoke(action, this.params(taskId, hintId), {} as Dictionary<Object>)
        .then((ar: ActionResultRepresentation) => {
          var obj = ar.result().object()!;
          return this.convertToHint(obj);
        })
        .catch((e: ErrorWrapper) => {
          console.log(`${e.title}:${e.description}`);
          return EmptyHintUserView;
        });
    });
  }

  gotoTask(taskId: number) {
    this.router.navigate([`/task/${taskId}`]);
  }
}
