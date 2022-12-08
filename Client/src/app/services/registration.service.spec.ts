import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { MenusRepresentation, UserRepresentation } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { first, Subject } from 'rxjs';

import { RegistrationService } from './registration.service';

describe('RegisteredService', () => {
  let service: RegistrationService;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;

  let authSubj = new Subject<boolean>();


  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['loginWithRedirect'], { isAuthenticated$: authSubj });
    contextServiceSpy = jasmine.createSpyObj('ContextService', ['getUser', 'getMenus'], {});
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    repLoaderSpy = jasmine.createSpyObj('RepLoaderService', ['populate']);


    TestBed.configureTestingModule({});
    service = new RegistrationService(authServiceSpy, contextServiceSpy, repLoaderSpy, routerSpy);

    contextServiceSpy.getUser.and.returnValue(Promise.resolve({ userName: () => "name" } as UserRepresentation));
    contextServiceSpy.getMenus.and.returnValue(Promise.resolve({  } as MenusRepresentation));
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should register', fakeAsync(() => {
    authSubj.next(true);
    tick();

    expect(contextServiceSpy.getMenus).toHaveBeenCalled();
    //expect(service.registered).toBeTrue();

  }));

  // it('should return registered bool for canActivate if set', fakeAsync(() => {
  //   authSubj.next(true);
  //   tick();

  //   expect(service.canActivate().pipe(first()).subscribe(b => expect(b).toBeTrue()));
  // }));

  it('should return registered bool for canActivate if set', fakeAsync(() => {
    authSubj.next(false);
    tick();

    expect(service.canActivate().pipe(first()).subscribe(b => expect(b).toBeFalse()));
  }));

  it('should naviaget to landing if registered undefined', fakeAsync(() => {

    expect(service.canActivate().pipe(first()).subscribe(b => expect(b).toBeFalse()));
    authSubj.next(false);
    tick();
    expect(routerSpy.navigate).toHaveBeenCalledOnceWith(['/landing']);
  }));

});
