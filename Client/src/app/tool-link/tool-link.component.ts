import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { restViewerLink } from '../constants/tooltips';

@Component({
  selector: 'app-tool-link',
  templateUrl: './tool-link.component.html',
  styleUrls: ['./tool-link.component.css']
})
export class ToolLinkComponent {

  constructor(private readonly router: Router) { }

  @Input()
  urlLink = "";

  onClick() {
    this.router.navigate([this.urlLink]);
  }

  get tooltip() {
    return restViewerLink;
  }
}
