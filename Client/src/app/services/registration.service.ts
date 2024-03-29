import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { BehaviorSubject, first, map, of } from 'rxjs';
import { UserService } from './user.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RegistrationService implements CanActivate {

  registered$ = new BehaviorSubject<boolean | null>(null);
  registered?: boolean;
  activeTaskId?: number;

  private setRegistered(registered?: boolean) {
    if (registered === true || registered === false) {
      this.registered = registered;
      this.registered$.next(registered);
    }
  }

  constructor(public auth: AuthService, private userService: UserService, private router: Router) {
    auth.isAuthenticated$.subscribe(b => {
      if (b) {
        this.isValidUser().pipe(first()).subscribe(valid => {
          if (valid) {
            this.userService.loadUser();
          }
          else {
            this.setRegistered(false);
          }
        });
      }
      else {
        this.setRegistered(false);
      }
    });

    userService.currentUser.subscribe(u => {
      this.activeTaskId = u.ActiveTaskId;
      this.setRegistered(u.Registered);
    });
  }

  static inviteCodeKey = "invitationCode";

  isValidUser() {
    return environment.blockAuth0Provider ? this.auth.user$.pipe(map(u => !!u && !!u.sub && !u.sub.startsWith('auth0'))) : of(true);
  }

  isLoggedOn() {
    return this.auth.isAuthenticated$;
  }

  getRegistered() {
    return this.userService.loadUser().then(u => {
      this.activeTaskId = u.ActiveTaskId;
      return u.Registered === true;
    });
  }

  canActivate() {
    if (this.registered === true) {
      return this.registered;
    }

    return this.getRegistered().then(b => {
      if (b === false) {
        return this.router.createUrlTree(['/landing']);
      }
      return b;
    });
  }

  canDeactivate(_c: object) {
    return true;
  }

  private callbackUrl(page?: string) {
    const url = (window as { location: { origin: string } }).location.origin;
    return `${url}/${page || 'landing'}`;
  }

  login() {
    this.auth.loginWithRedirect({ redirect_uri: this.callbackUrl(), scope: 'openid email profile', response_type: 'code' });
  }

  logout(page?: string) {
    this.auth.logout({ returnTo: this.callbackUrl(page) });
  }
}