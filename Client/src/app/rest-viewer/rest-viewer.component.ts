import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ConfigService,  } from '@nakedobjects/services';
import { catchError, first, of } from 'rxjs';
import { okButtonDisabledTooltip as goButtonDisabledTooltip, okButtonEnabledTooltip as goButtonEnabledTooltip, methodButtonDisabledTooltip, methodButtonEnabledTooltip, homeTooltip, nextUrlEnabledTooltip, nextUrlDisabledTooltip, previousUrlDisabledTooltip, previousUrlEnabledTooltip } from '../constants/tooltips';

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

  urlHistory: string[] = [];
  urlHistoryIndex = -1; 

  getUrl(url: string) {
    if (this.payload) {
      const encoded = encodeURI(this.payload);
      return `${url}?${encoded}`;
    }
    else {
      return url;
    }
  }

  addUrl(url : string, save: boolean) {
    this.currentUrl = url;
    if (save) {
      this.urlHistory.push(url);
      this.urlHistoryIndex = this.urlHistory.length -1;
    }
  }

  loadUrl(url: string, save: boolean) {
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
    
        if (this.message) {
          this.currentUrl = "";
          this.urlHistoryIndex = this.urlHistory.length;
        }
        else {
          this.addUrl(url, save);
          this.url = "";
          this.payload = "";
        }
      });
  }

  onGo() {
    this.loadUrl(this.url, true);
  }

  get disableGo() {
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

  get goTooltip() {
    return this.disableGo ? goButtonDisabledTooltip : goButtonEnabledTooltip;
  }

  get payloadPlaceholder() {
    return 'Copy/Paste from "arguments" in pane above e.g {"id": {"value": "1"}}';
  }

  onHome() {
    this.urlHistoryIndex = 0;
    const url = this.urlHistory[this.urlHistoryIndex];
    this.loadUrl(url, false);
  }

  ngOnInit(): void {
    this.loadUrl(this.home, true);
  }

  canPrevious() {
    return this.urlHistoryIndex > 0;
  }

  canNext() {
    return this.urlHistoryIndex < this.urlHistory.length -1;
  }

  onPrevious() {
    this.urlHistoryIndex--;
    const url = this.urlHistory[this.urlHistoryIndex];
    this.loadUrl(url, false);
  }

  onNext() {
    this.urlHistoryIndex++;
    const url = this.urlHistory[this.urlHistoryIndex];
    this.loadUrl(url, false);
  }

  get nextUrlTooltip() {
    return  this.canNext() ?  nextUrlEnabledTooltip : nextUrlDisabledTooltip;
  }

  get previousUrlTooltip() {
    return  this.canPrevious() ?  previousUrlEnabledTooltip : previousUrlDisabledTooltip;
  }
}
