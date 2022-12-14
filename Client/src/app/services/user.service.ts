import { Injectable } from '@angular/core';
import { Value, ActionResultRepresentation, IHateoasModel, DomainObjectRepresentation, InvokableActionMember, PropertyMember, EntryType, DomainServicesRepresentation } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { BehaviorSubject } from 'rxjs';
import { EmptyUserView, IUserView, UserView } from '../models/user-view';
import { convertTo } from './rep-helpers';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private contextService: ContextService, private repLoader: RepLoaderService) { }

  userAction?: InvokableActionMember;
  acceptInvitationAction?: InvokableActionMember;

  get currentUser() {
    return this.currentUserAsSubject;
  }

  private currentUserAsSubject = new BehaviorSubject<IUserView>(EmptyUserView);

  private convertToUser(rep: DomainObjectRepresentation) {
    return convertTo<IUserView>(new UserView(), rep);
  }

  getService() {
    return this.contextService.getServices()
      .then((services: DomainServicesRepresentation) => {
        const service = services.getService("Model.Functions.Services.UserService");
        return this.repLoader.populate(service);
      })
      .then((s: IHateoasModel) => s as DomainObjectRepresentation)
  }

  acceptInvitation(code: string) {
    return this.getAcceptInvitationAction().then(action => {
      return this.repLoader.invoke(action, { code: new Value(code) } as Dictionary<Value>, {} as Dictionary<Object>)
        .then(_ => {
          this.loadUser();
          return true;
        }) // success
        .catch(_ => {
          this.currentUserAsSubject.next(EmptyUserView);
          return false;
        });
    });
  }

  loadUser() {
    this.getUserAction().then(action => {
      this.repLoader.invoke(action, {} as Dictionary<Value>, {} as Dictionary<Object>)
        .then((ar: ActionResultRepresentation) => {
          var obj = ar.result().object()!;
          var user = this.convertToUser(obj);
          this.currentUserAsSubject.next(user);
        })
        .catch(_ => {
          this.currentUserAsSubject.next(EmptyUserView);
        });
    });
  }

  getUserAction() {
    return this.userAction
      ? Promise.resolve(this.userAction)
      : this.getAction("GetUser").then(action => this.userAction = action);
  }

  getAcceptInvitationAction() {
    return this.acceptInvitationAction
      ? Promise.resolve(this.acceptInvitationAction)
      : this.getAction("AcceptInvitation").then(action => this.acceptInvitationAction = action);
  }

  getAction(name: string) {
    return this.getService().then(service => service.actionMember(name) as InvokableActionMember);
  }
}
