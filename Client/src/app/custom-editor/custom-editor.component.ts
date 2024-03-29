import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Location } from '@angular/common';
import { FileService } from '../services/file.service';
import { IFileView } from '../models/file-view';
import { CompileServerService } from '../services/compile-server.service';

@Component({
  selector: 'app-custom-editor',
  templateUrl: './custom-editor.component.html',
  styleUrls: ['./custom-editor.component.css']
})
export class CustomEditorComponent implements OnInit, OnDestroy {

  constructor(private route: ActivatedRoute,
    private location: Location,
    private fileService: FileService,
    private compileService: CompileServerService) { }

  id = "";
  editContent = "";
  initialContent = "";
  lastSavedContent  = "";

  mime = "";

  name = "";

  sub1?: Subscription;

  sub2?: Subscription;

  file?: IFileView;

  changed = false;

  loaded = false;

  languages: string[] = [];

  get classes() {
    return `${this.selectedLanguage} ${this.mime.replace('/', '-')}`;
  }

  warning = "";

  modelChanged() {
    this.changed = true;
  }

  selectedLanguage = "csharp";

  onRadio(l: string) {
    this.selectedLanguage = l;
  }

  isChecked(l: string) {
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
    this.sub1 = this.route.paramMap.subscribe(pm => {
      this.id = pm.get('id') || "";
      if (this.id) {
        this.fileService.loadFile(this.id).then(file => {
          this.editContent = this.lastSavedContent = this.initialContent = file.Content;
          this.mime = file.Mime || 'text/plain';
          this.selectedLanguage = file.LanguageAlphaName || 'csharp';
          this.name = file.Name;
          this.loaded = true;
        });
      }
    });
    this.sub2 = this.compileService.languages$.subscribe(ll => this.languages = ll.map(i => i.AlphaName).filter((i, j, k) => k.indexOf(i) === j)); // distinct
  }

  hasChanges() {
    return this.editContent !== this.lastSavedContent;
  }

  onSave() {
    this.fileService.saveFile(this.id, this.editContent).then(b => {
      this.warning = "";
      if (b) {
        this.lastSavedContent = this.editContent;
      }
      else {
        this.warning = "Save failed!";
      }
    });
  }

  onSaveAndClose() {
    this.fileService.saveFile(this.id, this.editContent).then(b => {
      this.warning = "";
      if (b) {
        this.lastSavedContent = this.editContent;
        this.close();
      }
      else {
        this.warning = "Save failed!";
      }
    });
  }

  onCancel() {
    this.close();
  }

  onRevert() {
    this.editContent = this.lastSavedContent;
  }

  close() {
    const path = this.location.path();
    this.location.back();
    if (path === this.location.path()) {
      window.close();
    }
  }

  ngOnDestroy() {
    if (this.sub1) {
      this.sub1.unsubscribe();
    }
    if (this.sub2) {
      this.sub2.unsubscribe();
    }
  }
}
