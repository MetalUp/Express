import { Injectable } from '@angular/core';
import { Value, ActionResultRepresentation, DomainObjectRepresentation, InvokableActionMember } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { BehaviorSubject } from 'rxjs';
import { EmptyUserView, IUserView, RegisteredUserView, UnregisteredUserView, UserView } from '../models/user-view';
import { convertTo, getAction, getService } from './rep-helpers';

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
    return convertTo<IUserView>(RegisteredUserView, rep);
  }

  // must be lambda function for 'this' binding
  getService = () => getService(this.contextService, this.repLoader, "Model.Functions.Services.UserService");
  
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
    return this.getUserAction().then(action => {
      return this.repLoader.invoke(action, {} as Dictionary<Value>, {} as Dictionary<Object>)
        .then((ar: ActionResultRepresentation) => {
          var obj = ar.result().object();
          var user = obj ? this.convertToUser(obj) : UnregisteredUserView;
          this.currentUserAsSubject.next(user);
          return user;
        })
        .catch(_ => {
          this.currentUserAsSubject.next(EmptyUserView);
          return EmptyUserView as IUserView;
        });
    })
      .catch(_ => {
        this.currentUserAsSubject.next(EmptyUserView);
        return EmptyUserView as IUserView;
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
    return getAction(this.getService, name);
  }
}
