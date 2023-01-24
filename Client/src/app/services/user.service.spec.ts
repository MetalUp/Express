import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { InvokableActionMember, DomainObjectRepresentation, MenusRepresentation, ActionResultRepresentation, DomainServicesRepresentation, Value } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { first } from 'rxjs';
import { EmptyUserView, UnregisteredUserView } from '../models/user-view';

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

  it('should get the user', fakeAsync(() => {

    service.userAction = {} as InvokableActionMember;

    const testAr = { result: () => ({ object: () => null }) } as ActionResultRepresentation;

    repLoaderSpy.invoke.and.returnValue(Promise.resolve(testAr));

    service.loadUser().then(o => expect(o).toBe(UnregisteredUserView));
    tick();

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.userAction, {} as Dictionary<Value>, {} as Dictionary<Object>);
  }));

  it('should accept the invitation', fakeAsync(() => {

    service.acceptInvitationAction = {} as InvokableActionMember;

    const testAr = { result: () => ({ object: () => null }) } as ActionResultRepresentation;

    repLoaderSpy.invoke.and.returnValue(Promise.resolve(testAr));

    service.acceptInvitation('test code').then(o => expect(o).toBe(false));
    tick();

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.acceptInvitationAction, { code: new Value('test code') } as Dictionary<Value>, {} as Dictionary<Object>);
  }));
});
