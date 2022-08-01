import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'ile-client';

  constructor(private route: ActivatedRoute) { }

  language: string = '';

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.language = params['language'];
    });
  }
}
