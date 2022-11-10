
import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { ContextService } from '@nakedobjects/services';
import { AuthService } from '@auth0/auth0-angular';
import { of, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RegisteredService implements CanActivate {
  
  registered$ = new Subject<boolean>();
  private registered? : boolean;

  private setRegistered(registered : boolean) {
    this.registered = registered;
    this.registered$.next(registered);
  }

  constructor(private auth: AuthService, private contextService: ContextService) {
    
    auth.isAuthenticated$.subscribe(b => {
      if (b) {
          this.contextService.getUser()
            .then(u => {
              this.setRegistered (!!u.userName());
            })
            .catch(e => {
              this.setRegistered(false);
            })
      }
      else {
        this.setRegistered(false);
      }
    })
  }

  isLoggedOn() {
    return this.auth.isAuthenticated$
  }

  canActivate() {
    if (this.registered === true || this.registered === false){
      return of(this.registered);
    }
    return this.registered$;
  }

  canDeactivate(c : any) {
    return true;
  }

  login() {
    const url = (window as any).location.origin;
    const callbackUrl = `${url}/landing`;
    this.auth.loginWithRedirect({redirect_uri : callbackUrl,  scope: 'openid email profile', response_type: 'code'});
  } 

  logout() {
    this.auth.logout();
  } 
}