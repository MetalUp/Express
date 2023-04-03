import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { EmptyTaskUserView, ITaskUserView, TaskUserView } from '../models/task-user-view';
import { EmptyHintUserView, HintUserView, IHintUserView } from '../models/hint-user-view';
import { Subject } from 'rxjs';
import { ContextService, ErrorWrapper, RepLoaderService } from '@nakedobjects/services';
import { ActionResultRepresentation, DomainObjectRepresentation, DomainServicesRepresentation, IHateoasModel, InvokableActionMember, Value } from '@nakedobjects/restful-objects';
import { Dictionary } from 'lodash';
import { CodeUserView, EmptyCodeUserView, ICodeUserView } from '../models/code-user-view';
import { convertTo } from './rep-helpers';

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
      });
  }

  taskAccess?: DomainObjectRepresentation;

  get currentTask() {
    return this.currentTaskAsSubject;
  }

  private currentTaskAsSubject = new Subject<ITaskUserView>();

  private convertToTask(rep: DomainObjectRepresentation, id: number) {
    return convertTo<ITaskUserView>(new TaskUserView(id), rep);
  }

  private convertToHint(rep: DomainObjectRepresentation) {
    return convertTo<IHintUserView>(new HintUserView(), rep);
  }

  private convertToCode(rep: DomainObjectRepresentation) {
    return convertTo<ICodeUserView>(new CodeUserView(), rep);
  }

  params(taskId: number, currentHintNo?: number, codeVersion?: number) {
    const params = { "taskId": new Value(taskId) } as Dictionary<Value>;

    if (currentHintNo != undefined) {
      params['hintNumber'] = new Value(currentHintNo);
    }

    if (codeVersion != undefined) {
      params['codeVersion'] = new Value(codeVersion);
    }

    return params;
  }

  loadTask(taskId: number) {

    this.getService().then(s => {
      const action = s.actionMember("GetTask") as InvokableActionMember;

      this.repLoader.invoke(action, this.params(taskId), {})
        .then((ar: ActionResultRepresentation) => {
          const obj = ar.result().object()!;
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
          const obj = ar.result().object()!;
          return this.convertToHint(obj);
        })
        .catch((e: ErrorWrapper) => {
          console.log(`${e.title}:${e.description}`);
          return EmptyHintUserView as IHintUserView;
        });
    });
  }

  loadCode(taskId: number, version: number) {

    return this.getService().then(s => {
      const action = s.actionMember("GetCodeVersion") as InvokableActionMember;

      return this.repLoader.invoke(action, this.params(taskId, undefined, version), {} as Dictionary<Object>)
        .then((ar: ActionResultRepresentation) => {
          const obj = ar.result().object()!;
          return this.convertToCode(obj);
        })
        .catch((e: ErrorWrapper) => {
          console.log(`${e.title}:${e.description}`);
          return EmptyCodeUserView as ICodeUserView;
        });
    });
  }

  gotoTask(taskId: number) {
    this.router.navigate([`/task/${taskId}`]);
  }
}
