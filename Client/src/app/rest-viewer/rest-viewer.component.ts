import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ConfigService,  } from '@nakedobjects/services';
import { catchError, first, of } from 'rxjs';
import { okButtonDisabledTooltip, okButtonEnabledTooltip, methodButtonDisabledTooltip, methodButtonEnabledTooltip, homeTooltip } from '../constants/tooltips';

@Component({
  selector: 'app-rest-viewer',
  templateUrl: './rest-viewer.component.html',
  styleUrls: ['./rest-viewer.component.css']
})
export class RestViewerComponent implements OnInit {

  constructor(private readonly http: HttpClient, private readonly configService: ConfigService) { }

  private home = this.configService.config.appPath;

  currentUrl = this.home;
  url = "";

  content?: Object;

  payload = "";

  message = "";

  getUrl(url: string) {
    if (this.payload) {
      const encoded = encodeURI(this.payload);
      return `${url}?${encoded}`;
    }
    else {
      return url;
    }
  }

  loadUrl(url: string) {
    this.message = "";
    this.http.request('get', this.getUrl(url), { responseType: 'json' })
      .pipe(first())
      .pipe(catchError((e) => {
        if (e instanceof HttpErrorResponse){
          this.message = `${e.message}`;
        }
        else {
          this.message = "An unknown error occurred";
        }
        return of(new Object());
      }))
      .subscribe(b => {
        this.content = b;
        this.currentUrl = this.message ? "" :  url;
        this.url = "";
        this.payload = "";
      });
  }

  onOk() {
    this.loadUrl(this.url);
  }

  get disableOk() {
    return !(this.url.startsWith(this.home));
  }

  get methodEnabledTooltip() {
    return methodButtonEnabledTooltip;
  }

  get methodDisabledTooltip() {
    return methodButtonDisabledTooltip;
  }

  get homeTooltip() {
    return homeTooltip;
  }

  get okTooltip() {
    return this.disableOk ? okButtonDisabledTooltip : okButtonEnabledTooltip;
  }

  get payloadPlaceholder() {
    return 'Copy/Paste from pane on left e.g {"id": {"value": "1"}}';
  }

  onHome() {
    this.loadUrl(this.home);
  }

  ngOnInit(): void {
    this.loadUrl(this.home);
  }
}
