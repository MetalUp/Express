import { Injectable } from '@angular/core';
import { Value, ActionResultRepresentation, DomainObjectRepresentation, InvokableActionMember } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { EmptyFileView, FileView, IFileView } from '../models/file-view';
import { ErrorService } from './error.service';
import { convertTo, getAction, getService } from './rep-helpers';

@Injectable({
  providedIn: 'root'
})
export class FileService {

  constructor(private contextService: ContextService, private repLoader: RepLoaderService, private errorService: ErrorService) { }

  fileAction?: InvokableActionMember;
  saveAction?: InvokableActionMember;

  private convertToFile(rep: DomainObjectRepresentation) {
    return convertTo<IFileView>(new FileView(), rep);
  }

  // must be lambda function for 'this' binding
  getService = () => getService(this.contextService, this.repLoader, "Model.Services.FileService");

  saveFile(id: string, content: string) {
    return this.getSaveAction().then(action => {
      return this.repLoader.invoke(action, { id: new Value(id), content: new Value(content) } as Dictionary<Value>, {} as Dictionary<object>)
        .then(_ => true) // success
        .catch(_ => false);
    });
  }

  loadFile(id: string) {
    this.errorService.clearError();
    return this.getFileAction()
      .then(action => {
        return this.repLoader.invoke(action, { id: new Value(id) } as Dictionary<Value>, {} as Dictionary<object>)
          .then((ar: ActionResultRepresentation) => {
            const obj = ar.result().object();
            return obj ? this.convertToFile(obj) : EmptyFileView;
          })
          .catch(e => {
            this.errorService.addError(e);
            return EmptyFileView as IFileView;
          });
      })
      .catch(e => {
        this.errorService.addError(e);
        return EmptyFileView as IFileView;
      });
  }

  getFileAction() {
    return this.fileAction
      ? Promise.resolve(this.fileAction)
      : this.getAction("GetFile").then(action => this.fileAction = action);
  }

  getSaveAction() {
    return this.saveAction
      ? Promise.resolve(this.saveAction)
      : this.getAction("SaveFile").then(action => this.saveAction = action);
  }

  getAction(name: string) {
    return getAction(this.getService, name);
  }
}
