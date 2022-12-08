import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { AuthService } from '@auth0/auth0-angular';
import { first, of, pipe, Subject } from 'rxjs';
import { ActionResultRepresentation, DomainObjectRepresentation, IHateoasModel, InvokableActionMember, MenusRepresentation, Value } from '@nakedobjects/restful-objects';
import { Dictionary } from 'lodash';

@Injectable({
  providedIn: 'root'
})
export class RegistrationService implements CanActivate {

  registered$ = new Subject<boolean>();
  registered?: boolean;

  private setRegistered(registered: boolean) {
    this.registered = registered;
    this.registered$.next(registered);
  }

  constructor(public auth: AuthService,  private contextService: ContextService,  private repLoader: RepLoaderService, private router: Router) {
    auth.isAuthenticated$.subscribe(b => {
      if (b) {
        this.refreshRegistration();
      }
      else {
        this.setRegistered(false);
      }
    })
  }

  static inviteCodeKey = "invitationCode"

  refreshRegistration() {
    this.getUser()
      .then(u => {
        this.setRegistered(u === 1);
      })
      .catch(e => {
        this.setRegistered(false);
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

  getUser() {
    return this.getAction().then(action => {
      return this.repLoader.invoke(action, {  } as Dictionary<Value>, {} as Dictionary<Object>)
        .then((ar : ActionResultRepresentation)  =>  ar.result().object()?.propertyMember('Status').value().scalar() as number) // success
        .catch(_ => null);
    });
  }

  getMenu() {
    return this.contextService.getMenus()
      .then((menus: MenusRepresentation) => {
        const menu = menus.getMenu("Users");
        return this.repLoader.populate(menu);
      })
      .then((s: IHateoasModel) => s as DomainObjectRepresentation)
  }

  getAction() {
    return this.getMenu().then(menu => menu.actionMember("Me") as InvokableActionMember);
  }

}