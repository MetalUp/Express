import { Injectable } from '@angular/core';
import { Value, ActionResultRepresentation, IHateoasModel, DomainObjectRepresentation, InvokableActionMember, DomainServicesRepresentation } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { EmptyFileView, FileView, IFileView } from '../models/file-view';
import { convertTo } from './rep-helpers';

@Injectable({
  providedIn: 'root'
})
export class FileService {

  constructor(private contextService: ContextService, private repLoader: RepLoaderService) { }

  fileAction?: InvokableActionMember;
  saveAction?: InvokableActionMember;

  private convertToFile(rep: DomainObjectRepresentation) {
    return convertTo<IFileView>(new FileView(), rep);
  }

  getService() {
    return this.contextService.getServices()
      .then((services: DomainServicesRepresentation) => {
        const service = services.getService("Model.Functions.Services.FileService");
        return this.repLoader.populate(service);
      })
      .then((s: IHateoasModel) => s as DomainObjectRepresentation)
  }

  saveFile(id: string, content: string) {
    return this.getSaveAction().then(action => {
      return this.repLoader.invoke(action, { id: new Value(id), content: new Value(content) } as Dictionary<Value>, {} as Dictionary<Object>)
        .then(_ => {
          return true;
        }) // success
        .catch(_ => {
          return false;
        });
    });
  }

  loadFile(id: string) {
    return this.getFileAction().then(action => {
      return this.repLoader.invoke(action, { id: new Value(id) } as Dictionary<Value>, {} as Dictionary<Object>)
        .then((ar: ActionResultRepresentation) => {
          var obj = ar.result().object();
          return obj ? this.convertToFile(obj) : EmptyFileView;
        })
        .catch(_ => {
          return EmptyFileView as IFileView;
        });
    })
      .catch(_ => {
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
    return this.getService().then(service => service.actionMember(name) as InvokableActionMember);
  }
}
