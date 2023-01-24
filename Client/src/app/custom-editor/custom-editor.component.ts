import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { ContextService, InteractionMode } from '@nakedobjects/services';
import { DomainObjectRepresentation, InvokableActionMember, ObjectIdWrapper, Value } from '@nakedobjects/restful-objects';
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
              private contextService : ContextService) { }

  editContent: string = "";

  mime: string = "";

  sub?: Subscription;

  file?: DomainObjectRepresentation;

  get languages() {
    return ["arm", "java", "vb", "python", "csharp"];
  } 

  get classes() {
    return `${this.selectedLanguage} ${this.mime.replace('/', '-')}`;
  }

  selectedLanguage = "csharp";

  onRadio(l: string){
    this.selectedLanguage = l;
  }

  isChecked(l: string){
    return l === this.selectedLanguage ? "checked" : "";
  }

  get isHtmlEdit() {
    return this.mime === "text/html";
  }
  
  get paneSize() {
    return "pane-size-x-large";
  }

  get columns() {
    return this.isHtmlEdit ? "two-columns" : "one-column";
  }

  ngOnInit(): void {
    this.sub = this.route.paramMap.subscribe(pm => {
      const id = pm.get('id') || "";
      if (id) {
        this.contextService.getObject(1, ObjectIdWrapper.fromRaw("Model.Types.File", id, "--"), InteractionMode.View).then(o => {
          this.file = o;
          const action = this.file.actionMember("EditContentAsString") as InvokableActionMember;
          this.editContent = action.parameters()["content"].default().toValueString();
          this.mime = this.file.propertyMember("Mime").value().toValueString();
        })
      }
    })
  }

  onSave() {
    const action = this.file?.actionMember("EditContentAsString") as InvokableActionMember;
    
    const map = {} as Dictionary<Value>;

    map["content"] = new Value(this.editContent);

    this.contextService.invokeAction(action, map, 1, 1, false).then(ar => {
      this.location.back();
    });
  }

  onCancel() {
    this.location.back()
  }

  ngOnDestroy() {
    if (this.sub){
      this.sub.unsubscribe();
    }
  }
}
