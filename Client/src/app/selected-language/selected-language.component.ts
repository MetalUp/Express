import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { JobeServerService } from '../services/jobe-server.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-selected-language',
  templateUrl: './selected-language.component.html',
  styleUrls: ['./selected-language.component.css']
})
export class SelectedLanguageComponent implements OnInit, OnDestroy {

  constructor(private jobeServer: JobeServerService, private route: ActivatedRoute) { }

  languages: Array<[string, string]> = [];

  get selectedLanguage() {
    return this.jobeServer.selectedLanguage;
  }

  private checkLanguage() {
    if (this.selectedLanguage && this.languages.length > 0) {
      // check language is supported if not leave empty
      if (!this.languages.map(l => l[0]).includes(this.selectedLanguage)) {
        this.jobeServer.selectedLanguage = '';
      }
    }
  }

  private sub1?: Subscription;
  private sub2?: Subscription;

  ngOnInit(): void {
    this.sub1 = this.jobeServer.get_languages().subscribe(supportedLanguages => {
      this.languages = supportedLanguages;
      this.checkLanguage()
    });
    this.sub2 = this.route.queryParams.subscribe(params => {
      this.jobeServer.selectedLanguage = params['language'];
      this.checkLanguage()
    });
  }

  ngOnDestroy(): void {
    if (this.sub1) {
      this.sub1.unsubscribe();
    }
    if (this.sub2) {
      this.sub2.unsubscribe();
    }
  }
}
