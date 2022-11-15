import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { EmptyTaskUserView, ITaskUserView, TaskUserView } from '../models/task';
import { Hint, IHint } from '../models/hint';
import { Subject } from 'rxjs';
import { ContextService, ErrorWrapper, RepLoaderService } from '@nakedobjects/services';
import * as Ro from '@nakedobjects/restful-objects';
import { CollectionRepresentation, DomainObjectRepresentation, DomainServicesRepresentation, EntryType, IHateoasModel, InvokableActionMember, Value } from '@nakedobjects/restful-objects';
import { Dictionary } from 'lodash';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  
  constructor(private router: Router,
    private contextService: ContextService,
    private repLoader: RepLoaderService) { 
  }

  private getService() {
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

  taskAccess?: Ro.DomainObjectRepresentation;

  get currentTask() {
    return this.currentTaskAsSubject;
  }

  private currentTaskAsSubject = new Subject<ITaskUserView>()

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

  private setPropertyValue(toObj: any, member: Ro.PropertyMember) {
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

  private setCollectionValue(toObj: any, member: Ro.CollectionMember) {
    const details = member.getDetails();

    if (details) {
      this.repLoader.populate(details, true).then(d => {
        const pDetails = d as CollectionRepresentation;
        const links = pDetails.value();
        if (links && links.length > 0) {
          const items: IHint[] = [];
          var count = 0;
          for (const l of links) {
            this.repLoader.retrieveFromLink(l)
              .then((o: any) => {
                const hint = this.convertToHint(o);
                items[hint.Number - 1] = hint;
                count++;
                if (count === links.length) {
                  // only add hints once all have been loaded
                  toObj[member.collectionId()] = items;
                }
              })
              .catch(_ => console.log(`hints failed to load`));
          }
        }
      });
    }
  }

  private convertTo<T extends ITaskUserView | IHint>(toObj: TaskUserView | Hint, rep: DomainObjectRepresentation) {
    if (rep && Object.keys(rep.propertyMembers()).length > 0) {
      const pMembers = rep.propertyMembers()
      for (const k in pMembers) {
        const member = pMembers[k];
        this.setPropertyValue(toObj, member);
      }
      if (toObj instanceof TaskUserView) {
        // only get collections on task 
        const cMembers = rep.collectionMembers()
        for (const k in cMembers) {
          const member = cMembers[k];
          this.setCollectionValue(toObj, member);
        }
      }
    }
    return toObj as T;
  }

  private convertToTask(rep: DomainObjectRepresentation, id: number) {
    return this.convertTo<ITaskUserView>(new TaskUserView(id), rep);
  }

  private convertToHint(rep: DomainObjectRepresentation) {
    return this.convertTo<IHint>(new Hint(), rep);
  }

  private params(taskId: string, currentHintNo: string) {
    return { "taskId": new Value(taskId), "currentHintNo": new Value(currentHintNo) } as Dictionary<Value>
  }

  loadTask(taskId: string, hintId: string) {

    this.getService().then(s => {
      const action = s.actionMember("GetTask") as InvokableActionMember;

      this.repLoader.invoke(action, this.params(taskId, hintId), {} as Dictionary<Object>)
        .then((ar: Ro.ActionResultRepresentation) => {
          var obj = ar.result().object()!;
          const task = this.convertToTask(obj, parseInt(taskId));
          this.currentTaskAsSubject.next(task);
        })
        .catch((e: ErrorWrapper) => {
          console.log(`${e.title}:${e.description}`);
          this.currentTaskAsSubject.next(EmptyTaskUserView);
        });
    });
  }

  gotoTask(taskId: number, hintNo: number) {
    this.router.navigate([`/task/${taskId}-${hintNo}`]);
  }


  getFile(urlAndMediaType: [string, string]) {
    return this.repLoader.getFile(urlAndMediaType[0], urlAndMediaType[1], true)
      .then(b => b.text())
      .catch((e: ErrorWrapper) => {
        //console.log(`${e.title}:${e.description}`);
        return "";
      });
  }
}
