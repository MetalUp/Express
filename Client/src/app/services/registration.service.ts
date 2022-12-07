
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { ContextService } from '@nakedobjects/services';
import { AuthService } from '@auth0/auth0-angular';
import { first, of, pipe, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RegistrationService implements CanActivate {
  
  registered$ = new Subject<boolean>();
  registered? : boolean;

  private setRegistered(registered : boolean) {
    this.registered = registered;
    this.registered$.next(registered);
  }

  constructor(public auth: AuthService, private contextService: ContextService, private router: Router) {
    
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
    if (this.registered === true || this.registered === false) {
      return of(this.registered);
    }

    this.registered$.pipe(first()).subscribe(b => {
      if (b === false){
        this.router.navigate(['/landing']);
      }
    })

    return this.registered$;
  }

  canDeactivate(c : any) {
    return true;
  }

  private callbackUrl(page?: string) {
    const url = (window as any).location.origin;
    return `${url}/${page || 'landing'}`;
  }

  login(page? : string) {
    this.auth.loginWithRedirect({redirect_uri : this.callbackUrl(page),  scope: 'openid email profile', response_type: 'code'});
  } 

  logout() {
    this.auth.logout({returnTo: this.callbackUrl()});
  } 
}