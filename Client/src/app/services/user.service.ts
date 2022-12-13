import { Injectable } from '@angular/core';
import { Value, ActionResultRepresentation, MenusRepresentation, IHateoasModel, DomainObjectRepresentation, InvokableActionMember } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private contextService: ContextService, private repLoader: RepLoaderService) { }

  action?: InvokableActionMember;

  getUser() {
    return this.getAction().then(action => {
      return this.repLoader.invoke(action, {} as Dictionary<Value>, {} as Dictionary<Object>)
        .then((ar: ActionResultRepresentation) => ar.result().object()?.propertyMember('Status').value().scalar() as number) // success
        .catch(_ => null);
    });
  }

  getMenu() {
    return this.contextService.getMenus()
      .then((menus: MenusRepresentation) => {
        const menu = menus.getMenu("Users");
        return this.repLoader.populate(menu);
      })
      .then((s: IHateoasModel) => s as DomainObjectRepresentation)
  }

  getAction() {
    return this.action
      ? Promise.resolve(this.action)
      : this.getMenu().then(menu => this.action = menu.actionMember("Me") as InvokableActionMember);
  }
}
