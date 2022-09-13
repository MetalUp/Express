import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthService, ContextService } from '@nakedobjects/services';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {
  private user?: string;

  constructor(private contextService: ContextService) { }

  get userName() {
    return this.user;
  }

  ngOnInit(): void {
    this.contextService.getUser().then(u => this.user = u.userName() || "Guest" ).catch(e => this.user = "Unknown");
  }
}
