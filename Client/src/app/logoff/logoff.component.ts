import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';
import { ConfigService, ContextService } from '@nakedobjects/services';

@Component({
    selector: 'app-logoff',
    templateUrl: 'logoff.component.html',
    styleUrls: ['logoff.component.css']
})
export class LogoffComponent implements OnInit {

    constructor(
        private readonly context: ContextService,
        private readonly authService: AuthService,
        readonly configService: ConfigService,
        private readonly location: Location,
    ) { }

    userId: string = "";

    userIsLoggedIn() {
        return this.authService.isAuthenticated$;
    }

    cancel() {
        this.location.back();
    }

    logoff() {
        this.authService.logout();
    }

    ngOnInit() {
        this.context.getUser().then(u => this.userId = u.userName() || 'Unknown');
    }
}

