import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ContextService } from '@nakedobjects/services';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.css']
})
export class LandingComponent implements OnInit {

  constructor(private contextService: ContextService, private router : Router) { }

  ngOnInit(): void {

    this.contextService.getUser()
    .then(u => {
      if (u.userName()) {
        // known user go to home
        this.router.navigate([`/dashboard`]);
      }
      else {
        // go to task
        this.router.navigate([`/task`]);
      }
    })
    .catch(e => {
      // something wrong go to task
      this.router.navigate([`/task`]);
    });
  }

}
