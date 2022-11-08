import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RegisteredService } from '../services/registered.service';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit {

  constructor(public registeredService: RegisteredService, private router : Router) { }

  userChecked = false;
  
  get userLoggedOn() {
    return this.registeredService.isLoggedOn();
  }

  ngOnInit(): void {
    this.registeredService.isRegistered()
    .then(b => {
      if (b) {
        // known user go to home
        this.router.navigate([`/dashboard`]);
      }
      else {
        this.userChecked = true;
      }
    })
    .catch(e => {
      // something wrong  stay here
      this.userChecked = true;
    });
  }

}
