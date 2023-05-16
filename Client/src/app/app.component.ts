import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ConfigService, ErrorWrapper, RepLoaderService } from '@nakedobjects/services';
import { Subscription } from 'rxjs';
import { ErrorService } from './services/error.service';
import { RegistrationService } from './services/registration.service';
import { appVersion } from './app-version';
import { AppVersionHttpInterceptor } from './app-version.http-interceptor';

@Component({
    // tslint:disable-next-line:component-selector
    selector: 'app-root',
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.css']
})

export class AppComponent implements OnInit, OnDestroy, AfterViewInit {
    constructor(
        public readonly registeredService: RegistrationService,
        private readonly router: Router,
        private readonly repLoader: RepLoaderService,
        public readonly config: ConfigService,
        private readonly errorService: ErrorService) {
        
    }

    sub?: Subscription;
    sub1?: Subscription;

    loading = true;

    hasError = false;
    showDetails = false;

    errorTitle = "";
    errorDescription = "";
    errorCode = "";
    errorMessage = "";
    errorStacktrace: string[] = [];

    get classes() {
        return {
            'gemini': this.isDashboard(),
            'metalup': !this.isDashboard(),
            'custom-component': this.isCustomComponent(),
            'in-progress': this.loading,
            'not-in-progress': !this.loading
        };
    }

    get outletClass() {
        return this.isDashboard() ? "gemini" : "metalup";
    }

    get needsReload() {
        return localStorage.getItem(AppVersionHttpInterceptor.ReloadKey) && !this.hasError;
    }

    get pending() {
        return !this.config.loadingStatus && !this.hasError && !this.needsReload;
    }

    get loaded() {
        return this.config.loadingStatus === 1 && !this.hasError && !this.needsReload;
    }

    get loadFailed() {
        return this.config.loadingStatus === 2 && !this.hasError && !this.needsReload;
    }

    get clientVersion() {
        return appVersion;
    }

    get serverVersion() {
        return this.needsReload ? localStorage.getItem(AppVersionHttpInterceptor.ReloadKey) : "";
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

    onContinue() {
        localStorage.removeItem(AppVersionHttpInterceptor.ReloadKey);
        const w = window as { location: { origin: string, href: string } };
        const landing = `${w.location.origin}/landing`;
        w.location.href = landing;
    }

    onHome() {
        const w = window as { location: { origin: string, href: string } };
        const home = w.location.origin;
        w.location.href = home;
    }

    onDetails() {
        this.showDetails = true;
    }

    ngOnInit(): void {
        this.loading = false;
        this.sub = this.repLoader.loadingCount$.subscribe(t => {
            this.loading = t > 0;
        });

        this.sub1 = this.errorService.currentError.subscribe(e => {
            if (e == ErrorService.NoError) {
                this.hasError = false;
            }
            else if (e instanceof ErrorWrapper) {
                this.hasError = true;
                this.errorTitle = e.title;
                this.errorDescription = e.description;
                this.errorCode = e.errorCode;
                this.errorMessage = e.message;
                this.errorStacktrace = e.stackTrace;
            }
            else if (e) {
                const ea = e as any; 
                this.hasError = true;
                this.errorTitle = "Error";
                this.errorMessage = ea.message || "No message" ;
                this.errorStacktrace = ea.stackTrace || "No Stack Trace";
            }
        });
    }

    ngOnDestroy(): void {
        if (this.sub) {
            this.sub.unsubscribe();
        }
        if (this.sub1) {
            this.sub1.unsubscribe();
        }
    }

    @ViewChild('viewArea') 
    viewArea?: ElementRef;

    ngAfterViewInit() {
        this.viewArea!.nativeElement.closest('body').className = "not-in-progress";
    }
}
