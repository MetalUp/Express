import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-tool-link',
  templateUrl: './tool-link.component.html',
  styleUrls: ['./tool-link.component.css']
})
export class ToolLinkComponent {

  constructor(private readonly router: Router) { }

  @Input()
  urlLink: string = "";

  onClick() {
    this.router.navigate([this.urlLink]);
  }
}
