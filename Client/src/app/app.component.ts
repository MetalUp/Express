import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'ile-client';

  constructor(private route: ActivatedRoute) { }

  language: string = '';


  private sub?: Subscription;

  ngOnInit(): void {
    this.sub = this.route.queryParams.subscribe(params => {
      this.language = params['language'];
    });
  }

  ngOnDestroy(): void {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }
}
