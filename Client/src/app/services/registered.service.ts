
import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { AuthService, ContextService } from '@nakedobjects/services';

@Injectable({
  providedIn: 'root'
})
export class RegisteredService implements CanActivate {

  constructor(private auth: AuthService, private contextService : ContextService) { }

  public registered = false;

  isRegistered() {
    return this.canActivate();
  }

  isLoggedOn() {
    return this.auth.canActivate();
  }

  canActivate() {
    if (this.auth.canActivate()) {
      if (this.registered) {
        return Promise.resolve(true);
      }
      return this.contextService.getUser()
        .then(u => this.registered = !!u.userName())
        .catch(e => this.registered = false);
    }
    this.registered = false;
    return Promise.resolve(false);
  }

  canDeactivate(c : any) {
    return this.auth.canDeactivate(c);
  }

  login() {
    this.auth.login();
  } 

  logout() {
    this.auth.logout();
  } 

}