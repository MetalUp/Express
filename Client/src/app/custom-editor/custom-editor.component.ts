import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { ContextService, InteractionMode, RepLoaderService } from '@nakedobjects/services';
import {  DomainObjectRepresentation, InvokableActionMember, ObjectIdWrapper } from '@nakedobjects/restful-objects';
import { Location } from '@angular/common';


@Component({
  selector: 'app-custom-editor',
  templateUrl: './custom-editor.component.html',
  styleUrls: ['./custom-editor.component.css']
})
export class CustomEditorComponent implements OnInit, OnDestroy {

  constructor(private route: ActivatedRoute, 
              private location : Location, 
              private repLoader: RepLoaderService,
              private contextService : ContextService) { }

  editContent: string = "";

  sub?: Subscription;

  file?: DomainObjectRepresentation;

  get languages() {
    return ["arm", "java", "vb", "python", "csharp"];
  } 

  selectedLanguage = "csharp";

  onRadio(l: string){
    this.selectedLanguage = l;
  }

  isChecked(l: string){
    return l === this.selectedLanguage ? "checked" : "";
  }

  
  get paneSize() {
    return "pane-size-x-large";
  }

  ngOnInit(): void {
    this.sub = this.route.paramMap.subscribe(pm => {
      const id = pm.get('id') || "";
      if (id) {
        this.contextService.getObject(1, ObjectIdWrapper.fromObjectId(id, "--"), InteractionMode.View).then(o => {
          this.file = o;
          const action = this.file.actionMember("EditContentAsString") as InvokableActionMember;
          this.editContent = action.parameters()["content"].default().toValueString();
        })
      }
    })
  }

  onSave() {}


  onCancel() {
    this.location.back()
  }

  modelChanged() {

  }

  ngOnDestroy() {
    if (this.sub){
      this.sub.unsubscribe();
    }
  }
}
