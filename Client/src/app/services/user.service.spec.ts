import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { InvokableActionMember, DomainObjectRepresentation, MenusRepresentation, ActionResultRepresentation, DomainServicesRepresentation } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { first } from 'rxjs';
import { EmptyUserView } from '../models/user-view';

import { UserService } from './user.service';

describe('UserService', () => {
  let service: UserService;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;

  contextServiceSpy = jasmine.createSpyObj('ConfigService', ['getServices'], { config: { appPath: 'testPath' } });
  repLoaderSpy = jasmine.createSpyObj('RepLoaderService', ['populate', 'invoke'])

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = new UserService(contextServiceSpy, repLoaderSpy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get the user action', fakeAsync(() => {
    const testAction = {} as InvokableActionMember;
    const testService = { actionMember: (s: string) => testAction } as unknown as DomainObjectRepresentation;
    const testServices = { getService: (s: string) => testService } as unknown as DomainServicesRepresentation;

    const tssp = Promise.resolve(testServices);
    const tsp = Promise.resolve(testService);

    contextServiceSpy.getServices.and.returnValue(tssp);
    repLoaderSpy.populate.and.returnValue(tsp);
  
    service.getUserAction();
    tick();

    expect(contextServiceSpy.getServices).toHaveBeenCalled();
    expect(repLoaderSpy.populate).toHaveBeenCalledWith(testService);
    tick();
    expect(service.userAction).toEqual(testAction);
  }));

  it('should get the acceptInvitation action', fakeAsync(() => {
    const testAction = {} as InvokableActionMember;
    const testService = { actionMember: (s: string) => testAction } as unknown as DomainObjectRepresentation;
    const testServices = { getService: (s: string) => testService } as unknown as DomainServicesRepresentation;

    const tssp = Promise.resolve(testServices);
    const tsp = Promise.resolve(testService);

    contextServiceSpy.getServices.and.returnValue(tssp);
    repLoaderSpy.populate.and.returnValue(tsp);
  
    service.getAcceptInvitationAction();
    tick();

    expect(contextServiceSpy.getServices).toHaveBeenCalled();
    expect(repLoaderSpy.populate).toHaveBeenCalledWith(testService);
    tick();
    expect(service.acceptInvitationAction).toEqual(testAction);
  }));

  // it('should load the user', fakeAsync(() => {
  //   const tar = { result: () => ({ "object": () => ({ propertyMember: (s: string) => ({ value: () => ({ scalar: () => "Test User" }) }) }) }) } as unknown as ActionResultRepresentation;  

  //   repLoaderSpy.invoke.and.returnValue(Promise.resolve(tar));

  //   const testAction = {} as InvokableActionMember;
   
  //   service.userAction = testAction;

    

  //   service.loadUser();
  //   service.currentUser.pipe(first()).subscribe(u => expect(u.DisplayName).toBe(""));
  //   service.currentUser.pipe(first()).subscribe(u => expect(u.DisplayName).toBe("Test Users"));
  //   tick();
    
  //   expect(repLoaderSpy.invoke).toHaveBeenCalledWith(testAction, jasmine.objectContaining({}),  jasmine.objectContaining({}));
    

  // }));
});
