import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService, ConfigService, RepLoaderService } from '@nakedobjects/services';
import { Subscription } from 'rxjs';

@Component({
    // tslint:disable-next-line:component-selector
    selector: 'app-root',
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.css']
})

export class AppComponent implements OnInit, OnDestroy {
    constructor(public readonly auth: AuthService,
        private readonly router: Router,
        private readonly repLoader: RepLoaderService,
        public readonly config: ConfigService) {
        auth.handleAuthentication();
    }

    sub?: Subscription;

    loading = false;

    get classes() {
        return {
            'gemini': this.isDashboard(),
            'metalup': !this.isDashboard(),
            'in-progress': this.loading,
            'not-in-progress': !this.loading
        }
    }

    get outletClass() {
        return this.isDashboard() ? "gemini" : "metalup";
    }

    isDashboard() {
        const url = this.router.url;
        const segments = url.split('/');
        const [, mode] = segments;

        return mode === 'dashboard';
    }

    ngOnInit(): void {
        this.sub = this.repLoader.loadingCount$.subscribe(t => {
            this.loading = t > 0;
        })
    }

    ngOnDestroy(): void {
        if (this.sub) {
            this.sub.unsubscribe();
        }
    }
}
