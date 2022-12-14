import { Injectable } from '@angular/core';
import { Value, ActionResultRepresentation, MenusRepresentation, IHateoasModel, DomainObjectRepresentation, InvokableActionMember, PropertyMember, EntryType, DomainServicesRepresentation } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { BehaviorSubject } from 'rxjs';
import { EmptyUserView, IUserView, UserView } from '../models/user-view';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private contextService: ContextService, private repLoader: RepLoaderService) { }

  action?: InvokableActionMember;

  get currentUser() {
    return this.currentUserAsSubject;
  }

  private currentUserAsSubject = new BehaviorSubject<IUserView>(EmptyUserView);

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

  private convertTo(toObj: UserView, rep: DomainObjectRepresentation) {
    if (rep && Object.keys(rep.propertyMembers()).length > 0) {
      const pMembers = rep.propertyMembers()
      for (const k in pMembers) {
        const member = pMembers[k];
        this.setPropertyValue(toObj, member);
      }
    }
    return toObj;
  }


  getUser() {
    return this.getAction().then(action => {
      return this.repLoader.invoke(action, {} as Dictionary<Value>, {} as Dictionary<Object>)
        .then((ar: ActionResultRepresentation) => {
          var obj = ar.result().object()!;
          var user = this.convertTo(new UserView(), obj);
          this.currentUserAsSubject.next(user);
          return user;
        }) // success
        .catch(_ => {
          this.currentUserAsSubject.next(EmptyUserView);
          return EmptyUserView;
        });
    });
  }

  getService() {
    return this.contextService.getServices()
      .then((services: DomainServicesRepresentation) => {
        const service = services.getService("Model.Functions.Services.UserService");
        return this.repLoader.populate(service);
      })
      .then((s: IHateoasModel) => s as DomainObjectRepresentation)
  }

  getAction() {
    return this.action
      ? Promise.resolve(this.action)
      : this.getService().then(service => this.action = service.actionMember("GetUser") as InvokableActionMember);
  }
}
