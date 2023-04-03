import { Location } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ConfigService } from '@nakedobjects/services';
import { Subscription } from 'rxjs';
import { EmptyUserView } from '../models/user-view';
import { RegistrationService } from '../services/registration.service';
import { UserService } from '../services/user.service';

@Component({
    selector: 'app-logoff',
    templateUrl: 'logoff.component.html',
    styleUrls: ['logoff.component.css']
})
export class LogoffComponent implements OnInit, OnDestroy {

    constructor(
        private readonly userService: UserService,
        private readonly registeredService: RegistrationService,
        readonly configService: ConfigService,
        private readonly location: Location,
    ) { }

    user = EmptyUserView;

    private sub?: Subscription;

    userId = "";

    userIsLoggedIn() {
        return this.registeredService.auth.isAuthenticated$;
    }

    cancel() {
        this.location.back();
    }

    logoff() {
        this.registeredService.logout();
    }

    ngOnInit(): void {
        this.sub = this.userService.currentUser.subscribe(u =>
            this.user = u);
    }

    ngOnDestroy() {
        if (this.sub) {
            this.sub.unsubscribe();
        }
    }
}

