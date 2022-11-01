import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, ConfigService } from '@nakedobjects/services';

@Component({
    // tslint:disable-next-line:component-selector
    selector: 'app-root',
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.css']
})

export class AppComponent {
    constructor(public readonly auth: AuthService,
                private readonly router: Router,
                public readonly config: ConfigService) {
        auth.handleAuthentication();
     }

     get outletClass() {
        return this.isDashboard() ? "gemini" : "metalup";
     }

    isDashboard() {
        const url = this.router.url;
        const segments = url.split('/');
        const [,mode] =  segments;
        
        return mode === 'dashboard';
    }
}
