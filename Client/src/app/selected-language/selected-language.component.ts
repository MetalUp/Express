import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { JobeServerService } from '../services/jobe-server.service';

@Component({
  selector: 'app-selected-language',
  templateUrl: './selected-language.component.html',
  styleUrls: ['./selected-language.component.css']
})
export class SelectedLanguageComponent implements OnInit {

  constructor(private jobeServer: JobeServerService, private route: ActivatedRoute) { }

  languages: Array<[string, string]> = [];

  selectedLanguage: string = '';

  ngOnInit(): void {
    this.jobeServer.get_languages().subscribe(supportedLanguages => {
      this.languages = supportedLanguages;
      this.checkLanguage()
    });
    this.route.queryParams.subscribe(params => {
      this.selectedLanguage = params['language'];
      this.checkLanguage()
    });
  }

  private checkLanguage() {
    if (this.selectedLanguage && this.languages.length > 0) {
      // check language is supported if not leave empty
      if (!this.languages.map(l => l[0]).includes(this.selectedLanguage)) {
        this.selectedLanguage = '';
      }
    }
  }

}
