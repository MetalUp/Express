import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { ContextService, ErrorWrapper, InteractionMode, RepLoaderService } from '@nakedobjects/services';
import { ActionRepresentation, ActionResultRepresentation, DomainObjectRepresentation, InvokableActionMember, ObjectIdWrapper, Value } from '@nakedobjects/restful-objects';
import { Location } from '@angular/common';
import { Dictionary } from 'lodash';

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
    return ["arm", "vb", "python", "csharp"];
  } 

  selectedLanguage = "csharp";

  onRadio(l: string){
    this.selectedLanguage = l;
  }

  isChecked(l: string){
    return l === this.selectedLanguage ? "checked" : "";
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
