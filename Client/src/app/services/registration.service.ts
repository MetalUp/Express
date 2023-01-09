import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { first, of, Subject } from 'rxjs';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root'
})
export class RegistrationService implements CanActivate {

  registered$ = new Subject<boolean>();
  registered?: boolean;

  private setRegistered(registered?: boolean) {
    if (registered === true || registered === false) {
      this.registered = registered;
      this.registered$.next(registered);
    }
  }

  constructor(public auth: AuthService,  private userService: UserService, private router: Router) {
    auth.isAuthenticated$.subscribe(b => {
      if (b) {
        this.userService.loadUser();
      }
      else {
        this.setRegistered(false);
      }
    })

    userService.currentUser.subscribe(u => {
      this.setRegistered(u.Registered);
    });
  }

  static inviteCodeKey = "invitationCode"

  isLoggedOn() {
    return this.auth.isAuthenticated$
  }

  canActivate() {
    if (this.registered === true || this.registered === false) {
      return of(this.registered);
    }

    this.registered$.pipe(first()).subscribe(b => {
      if (b === false) {
        this.router.navigate(['/landing']);
      }
    })

    return this.registered$;
  }

  canDeactivate(c: any) {
    return true;
  }

  private callbackUrl(page?: string) {
    const url = (window as any).location.origin;
    return `${url}/${page || 'landing'}`;
  }

  login() {
    this.auth.loginWithRedirect({ redirect_uri: this.callbackUrl(), scope: 'openid email profile', response_type: 'code' });
  }

  logout(page?: string) {
    this.auth.logout({ returnTo: this.callbackUrl(page) });
  }
}