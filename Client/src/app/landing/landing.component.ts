import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { RegistrationService } from '../services/registration.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit, OnDestroy {

  constructor(public registeredService: RegistrationService, private router: Router) { }

  userChecked = false;
  sub?: Subscription;

  get userLoggedOn() {
    return this.registeredService.isLoggedOn();
  }

  ngOnInit(): void {
    this.sub = this.registeredService.registered$.subscribe(b => {
      if (b) {
        this.router.navigate(['/dashboard']);
      }
      this.userChecked = true;
    });
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
