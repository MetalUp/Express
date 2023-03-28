import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ConfigService, RepLoaderService } from '@nakedobjects/services';
import { Subscription } from 'rxjs';
import { RegistrationService } from './services/registration.service';

@Component({
    // tslint:disable-next-line:component-selector
    selector: 'app-root',
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.css']
})

export class AppComponent implements OnInit, OnDestroy {
    constructor(
        public readonly registeredService: RegistrationService,
        private readonly router: Router,
        private readonly repLoader: RepLoaderService,
        public readonly config: ConfigService) {
        
    }

    sub?: Subscription;

    loading = true;

    get classes() {
        return {
            'gemini': this.isDashboard(),
            'metalup': !this.isDashboard(),
            'custom-component': this.isCustomComponent(),
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

    isCustomComponent() {
        const url = this.router.url;
        const segments = url.split('/');
        const [, mode, subMode] = segments;

        return subMode === 'editor' || mode === 'roapiviewer';
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
