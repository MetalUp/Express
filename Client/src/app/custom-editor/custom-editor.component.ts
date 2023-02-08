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

  id?: string;

  editContent: string = "";
  initialContent: string = "";

  mime: string = "";

  name: string = "";

  sub1?: Subscription;

  sub2?: Subscription;

  file?: IFileView;

  changed: boolean = false;

  loaded: boolean = false;

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
          this.editContent = this.initialContent = file.Content;
          this.mime = file.Mime || 'text/plain';
          this.selectedLanguage = file.LanguageAlphaName || 'csharp';
          this.name = file.Name;
          this.loaded = true;
        })
      }
    })
    this.sub2 = this.compileService.languages$.subscribe(ll => this.languages = ll.map(i => i.AlphaName));
  }

  canSave() {
    return this.editContent !== this.initialContent;
  }

  onSave() {
    this.fileService.saveFile(this.id!, this.editContent).then(b => {
      this.warning = "";
      if (b) {
        this.location.back();
      }
      else {
        this.warning = "Save failed!";
      }
    });
  }

  onCancel() {
    this.location.back()
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
