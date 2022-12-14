import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { RepLoaderService } from '@nakedobjects/services';
import { first, Subject } from 'rxjs';
import { EmptyUserView, UserView } from '../models/user-view';
import { RegistrationService } from './registration.service';
import { UserService } from './user.service';

describe('RegisteredService', () => {
  let service: RegistrationService;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;

  let authSubj = new Subject<boolean>();

  let userSubj = new Subject<UserView>();


  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['loginWithRedirect'], { isAuthenticated$: authSubj });
    userServiceSpy = jasmine.createSpyObj('UserService', ['getUser'], {});
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    repLoaderSpy = jasmine.createSpyObj('RepLoaderService', ['populate']);


    TestBed.configureTestingModule({});
    service = new RegistrationService(authServiceSpy, userServiceSpy, routerSpy);

    userServiceSpy.getUser.and.returnValue(Promise.resolve({DisplayName : "Test User"}));
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should check registration - true', fakeAsync(() => {
    userServiceSpy.getUser.and.returnValue(Promise.resolve({DisplayName: "Test Name"}))

    authSubj.next(true);
    tick();

    expect(userServiceSpy.getUser).toHaveBeenCalled();
    expect(service.registered).toBeTrue();

  }));

  it('should check registration - false', fakeAsync(() => {
    userServiceSpy.getUser.and.returnValue(Promise.resolve(EmptyUserView))

    authSubj.next(true);
    tick();

    expect(userServiceSpy.getUser).toHaveBeenCalled();
    expect(service.registered).toBeFalse();

  }));

  it('should return registered bool for canActivate if set', fakeAsync(() => {
    authSubj.next(true);
    tick();

    expect(service.canActivate().pipe(first()).subscribe(b => expect(b).toBeTrue()));
  }));

  it('should return registered bool for canActivate if set', fakeAsync(() => {
    authSubj.next(false);
    tick();

    expect(service.canActivate().pipe(first()).subscribe(b => expect(b).toBeFalse()));
  }));

  it('should navigate to landing if registered undefined', fakeAsync(() => {

    expect(service.canActivate().pipe(first()).subscribe(b => expect(b).toBeFalse()));
    authSubj.next(false);
    tick();
    expect(routerSpy.navigate).toHaveBeenCalledOnceWith(['/landing']);
  }));

});
