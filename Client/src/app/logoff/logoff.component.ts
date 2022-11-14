import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { ConfigService, ContextService } from '@nakedobjects/services';
import { RegisteredService } from '../services/registered.service';

@Component({
    selector: 'app-logoff',
    templateUrl: 'logoff.component.html',
    styleUrls: ['logoff.component.css']
})
export class LogoffComponent implements OnInit {

    constructor(
        private readonly context: ContextService,
        private readonly registeredService: RegisteredService,
        readonly configService: ConfigService,
        private readonly location: Location,
    ) { }

    userId: string = "";

    userIsLoggedIn() {
        return this.registeredService.auth.isAuthenticated$;
    }

    cancel() {
        this.location.back();
    }

    logoff() {
        this.registeredService.logout();
    }

    ngOnInit() {
        this.context.getUser().then(u => this.userId = u.userName() || 'Unknown');
    }
}

