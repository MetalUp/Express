import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { RegistrationService } from '../services/registration.service';

@Component({
  selector: 'app-invitation',
  templateUrl: './invitation.component.html',
  styleUrls: ['./invitation.component.css']
})
export class InvitationComponent implements OnInit, OnDestroy {

  constructor(private route: ActivatedRoute, public registeredService: RegistrationService) { }

  private sub?: Subscription;

  showPage = false;

  ngOnInit(): void {
    this.sub = this.route.paramMap.subscribe(pm => {
      const code = pm.get('id') || "";
      if (code) {
        localStorage.setItem(RegistrationService.inviteCodeKey, code);
        this.registeredService.logout("invitation");
      }
      else {
        this.showPage = true;
      }
    });
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
