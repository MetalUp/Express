import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription, first } from 'rxjs';
import { RegistrationService } from '../services/registration.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit, OnDestroy {

  constructor(private router: Router,
              public registeredService: RegistrationService,
              private userService: UserService) { }

  userChecked = false;
  pendingStatus = true;
  sub1?: Subscription;
  sub2?: Subscription;
  failedInvite = false;
  invalidUser = false;

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
        const tid = this.registeredService.activeTaskId;
        const goToUrl = tid ? `/task/${tid}` : '/dashboard';

        this.router.navigate([goToUrl]);
      }
      else if (registered === false) {
        this.userChecked = true;
        this.pendingStatus = false;
      }
    });

    this.sub2 = this.registeredService.isLoggedOn().subscribe(loggedOn => {
      if (loggedOn) {
        this.registeredService.isValidUser().pipe(first()).subscribe(valid => {
          if (valid) {
            const code = this.invitationCode;
            this.clearCode();
            if (code) {
              this.userService.acceptInvitation(code).then(b => this.failedInvite = !b);
            }
          }
          this.invalidUser = !valid;
        });
      }
    });
  }

  ngOnDestroy(): void {
    if (this.sub1) {
      this.sub1.unsubscribe();
    }
  }
}
