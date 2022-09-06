import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { Subscription } from 'rxjs';
import { TaskService } from './services/task.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'ile-client';

  private sub?: Subscription;
  private isAuthenticated: boolean = false;

  constructor(private authService: AuthService) {

  }
  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
  ngOnInit(): void {
    this.sub = this.authService.isAuthenticated$.subscribe(b => this.isAuthenticated = b);
  }

  get isLoggedIn() {
    return this.isAuthenticated;
  }
}
