import { Component, OnDestroy, OnInit } from '@angular/core';
//import { AuthService } from '@auth0/auth0-angular';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit, OnDestroy {
  private user?: string;

  //constructor(private authService: AuthService) { }

  private sub?: Subscription;

  get userName() {
    return this.user;
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }

  ngOnInit(): void {
   
      this.user = 'Unknown';
  }
}
