import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-armlite',
  templateUrl: './armlite.component.html',
  styleUrls: ['./armlite.component.css']
})
export class ArmliteComponent {

  constructor() {
    (<any>window).Matomo.addTracker()
   }

  get armliteClass() {
    return "metalup-armlite"
  }


}
