import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Location } from '@angular/common';
import { FileService } from '../services/file.service';
import { IFileView } from '../models/file-view';

@Component({
  selector: 'app-custom-editor',
  templateUrl: './custom-editor.component.html',
  styleUrls: ['./custom-editor.component.css']
})
export class CustomEditorComponent implements OnInit, OnDestroy {

  constructor(private route: ActivatedRoute, 
              private location : Location, 
              private fileService : FileService) { }

  id?: string;

  editContent: string = "";

  mime: string = "";

  sub?: Subscription;

  file?: IFileView;

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
      this.id = pm.get('id') || "";
      if (this.id) {
        this.fileService.loadFile(this.id).then(file => {
          this.editContent = file.Content;
          this.mime = file.Mime || 'text/plain';
          this.selectedLanguage = file.LanguageAlphaName || 'csharp';
        })
      }
    })
  }

  onSave() {
    this.fileService.saveFile(this.id!, this.editContent).then(b => {
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
