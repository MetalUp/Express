import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ConfigService,  } from '@nakedobjects/services';
import { first } from 'rxjs';

@Component({
  selector: 'app-rest-viewer',
  templateUrl: './rest-viewer.component.html',
  styleUrls: ['./rest-viewer.component.css']
})
export class RestViewerComponent implements OnInit {

  constructor(private readonly http: HttpClient, private readonly configService: ConfigService) { }

  url = this.configService.config.appPath;

  content?: Object;

  ngOnInit(): void {
    this.http.request('get', this.url, {responseType : 'json'}).pipe(first()).subscribe(b => {
      this.content = b;
    });
  }
}
