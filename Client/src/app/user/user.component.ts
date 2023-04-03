import { Component,  OnDestroy,  OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { EmptyUserView } from '../models/user-view';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit, OnDestroy {
  private user = EmptyUserView;

  constructor(private userService: UserService) { }

  private sub? : Subscription;

  get userName() {
    return this.user.DisplayName;
  }

  ngOnInit(): void {
    this.sub = this.userService.currentUser.subscribe(u => this.user = u);
  }

  ngOnDestroy() {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
