import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpResponse
} from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { appVersion} from './app-version';
import { environment } from 'src/environments/environment';

@Injectable()
export class AppVersionHttpInterceptor implements HttpInterceptor {

  public static ReloadKey = "reload_key";

  private appV: string; 

  constructor() {
    this.appV = appVersion;
  }

  private triggerReload(version?: string) {
    if (environment.checkAppVersion && version && version !== this.appV) {
      localStorage.setItem(AppVersionHttpInterceptor.ReloadKey, version);
    }
  }

  private handleResponse(evt: HttpEvent<unknown>) {
    if (evt instanceof HttpResponse) {
      const headers = evt.headers;
      const ct = headers.get("Content-Type");
      const tokens = ct?.split(";") || [];

      const versionToken = tokens.find(t => t.trim().startsWith("version"));
      const version = versionToken?.split("=")[1];
      this.triggerReload(version);
    }
    return evt;
  }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(map(e => this.handleResponse(e)));
  }
}
