import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DomainObjectRepresentation, DomainServicesRepresentation, IHateoasModel, InvokableActionMember, MenusRepresentation, Value } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { Subscription } from 'rxjs';
import { RegistrationService } from '../services/registration.service';

@Component({
  selector: 'app-invitation',
  templateUrl: './invitation.component.html',
  styleUrls: ['./invitation.component.css']
})
export class InvitationComponent implements OnInit, OnDestroy {


  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private registeredService: RegistrationService,
    private contextService: ContextService,
    private repLoader: RepLoaderService) { }

  private sub1?: Subscription;
  private sub2?: Subscription;
  private sub3?: Subscription;
  private code?: string;
  private status: string = "Validating user..."

  get message() {
    return this.status;
  }

  ngOnInit(): void {
    this.sub1 = this.route.paramMap.subscribe(pm => {
      this.code = pm.get('id') || "";
    });

    this.sub2 = this.registeredService.registered$.subscribe(registered => {
      if (registered) {
        this.code = undefined;
        this.router.navigate(['/dashboard']);
      }
    });

    this.sub3 = this.registeredService.isLoggedOn().subscribe(loggedOn => {
      if (loggedOn) {
        this.status = "Checking Invitation Code..."
        this.acceptInvitation();
      }
      else {
        this.status = "Logging in user..."
        this.registeredService.login('invitation');
      }
    });
  }

  ngOnDestroy(): void {
    if (this.sub1) {
      this.sub1.unsubscribe();
    }

    if (this.sub2) {
      this.sub2.unsubscribe();
    }

    if (this.sub3) {
      this.sub3.unsubscribe();
    }
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

  acceptInvitation() {
    if (this.code === undefined) { return; }

    this.getAction().then(action => {
      if (this.code === undefined) { return; }

      this.repLoader.invoke(action, { code: new Value(this.code!) } as Dictionary<Value>, {} as Dictionary<Object>)
        .then(_ => this.code = undefined) // success
        .catch(_ => this.status = "Sorry this is not a valid invitation code");
    });
  }
}
