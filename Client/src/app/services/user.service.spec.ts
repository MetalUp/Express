import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { InvokableActionMember, DomainObjectRepresentation, MenusRepresentation, ActionResultRepresentation } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';

import { UserService } from './user.service';

describe('UserService', () => {
  let service: UserService;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;

  contextServiceSpy = jasmine.createSpyObj('ConfigService', ['getMenus'], { config: { appPath: 'testPath' } });
  repLoaderSpy = jasmine.createSpyObj('RepLoaderService', ['populate', 'invoke'])

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = new UserService(contextServiceSpy, repLoaderSpy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get the action', fakeAsync(() => {
    const testAction = {} as InvokableActionMember;
    const testMenu = { actionMember: (s: string) => testAction } as unknown as DomainObjectRepresentation;
    const testMenus = { getMenu: (s: string) => testMenu } as unknown as MenusRepresentation;

    const tssp = Promise.resolve(testMenus);
    const tsp = Promise.resolve(testMenu);

    contextServiceSpy.getMenus.and.returnValue(tssp);
    repLoaderSpy.populate.and.returnValue(tsp);
  
    service.getAction();
    tick();

    expect(contextServiceSpy.getMenus).toHaveBeenCalled();
    expect(repLoaderSpy.populate).toHaveBeenCalledWith(testMenu);
    tick();
    expect(service.action).toEqual(testAction);
  }));

  it('should get the user', fakeAsync(() => {
    const tar = { result: () => ({ "object": () => ({ propertyMember: (s: string) => ({ value: () => ({ scalar: () => 666 }) }) }) }) } as unknown as ActionResultRepresentation;  

    repLoaderSpy.invoke.and.returnValue(Promise.resolve(tar));

    const testAction = {} as InvokableActionMember;
   
    service.action = testAction;

    service.getUser().then(i => expect(i).toBe(666));
    tick();
    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(testAction, jasmine.objectContaining({}),  jasmine.objectContaining({}));
    

  }));
});
