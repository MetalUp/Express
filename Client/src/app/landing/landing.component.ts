import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DomainServicesRepresentation, IHateoasModel, DomainObjectRepresentation, InvokableActionMember, Value } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { Subscription } from 'rxjs';
import { RegistrationService } from '../services/registration.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit, OnDestroy {

  constructor(private router: Router,
              public registeredService: RegistrationService,
              private contextService: ContextService,
              private repLoader: RepLoaderService) { }

  userChecked = false;
  sub1?: Subscription;
  sub2?: Subscription;
  failedInvite = false;

  get userLoggedOn() {
    return this.registeredService.isLoggedOn();
  }

  get invitationCode() {
    return localStorage.getItem(RegistrationService.inviteCodeKey);
  }

  clearCode() {
    localStorage.removeItem(RegistrationService.inviteCodeKey);
  }

  ngOnInit(): void {
    this.sub1 = this.registeredService.registered$.subscribe(registered => {
      if (registered) {
        this.router.navigate(['/dashboard']);
      }
      this.userChecked = true;
    });

    this.sub2 = this.registeredService.isLoggedOn().subscribe(loggedOn => {
      if (loggedOn) {
        const code = this.invitationCode;
        this.clearCode();
        if (code) {
          this.acceptInvitation(code);
        }
      }
    });
  }

  getService() {
    return this.contextService.getServices()
      .then((services: DomainServicesRepresentation) => {
        const service = services.getService("Model.Functions.Services.InvitationAcceptance");
        return this.repLoader.populate(service);
      })
      .then((s: IHateoasModel) => s as DomainObjectRepresentation)
  }

  getAction() {
    return this.getService().then(service => service.actionMember("AcceptInvitation") as InvokableActionMember);
  }

  acceptInvitation(code: string) {
    this.getAction().then(action => {

      this.repLoader.invoke(action, { code: new Value(code) } as Dictionary<Value>, {} as Dictionary<Object>)
        .then(_ => this.registeredService.refreshRegistration()) // success
        .catch(_ => this.failedInvite = true);
    });
  }

  ngOnDestroy(): void {
    if (this.sub1) {
      this.sub1.unsubscribe();
    }
  }
}
