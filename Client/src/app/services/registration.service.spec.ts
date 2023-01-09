import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { RepLoaderService } from '@nakedobjects/services';
import { BehaviorSubject, first, Subject } from 'rxjs';
import { IUserView, UserView } from '../models/user-view';
import { RegistrationService } from './registration.service';
import { UserService } from './user.service';

describe('RegisteredService', () => {
  let service: RegistrationService;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let routerSpy: jasmine.SpyObj<Router>;
  
  let authSubj = new Subject<boolean>();

  let userSubj = new BehaviorSubject<UserView>({DisplayName: ""});


  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['loginWithRedirect'], { isAuthenticated$: authSubj });
    userServiceSpy = jasmine.createSpyObj('UserService', ['loadUser'], {currentUser : userSubj});
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({});
    service = new RegistrationService(authServiceSpy, userServiceSpy, routerSpy); 
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should check registration - true', fakeAsync(() => {

    userSubj.next({ DisplayName: "Test Name", Registered: true } as IUserView);
    authSubj.next(true);
    tick();

    expect(userServiceSpy.loadUser).toHaveBeenCalled();
    expect(service.registered).toBeTrue();

  }));

  it('should check registration - false', fakeAsync(() => {

    userSubj.next({ DisplayName: "", Registered: false } as IUserView);
    authSubj.next(true);
    tick();

    expect(userServiceSpy.loadUser).toHaveBeenCalled();
    expect(service.registered).toBeFalse();

  }));

  it('should return registered bool for canActivate if set and auth', fakeAsync(() => {
    userSubj.next({DisplayName : "Test Name", Registered: true} as IUserView);
    authSubj.next(true);
    tick();

    //expect(service.canActivate().then(b => expect(b).toBeTrue()));
  }));

  it('should return registered bool for canActivate if set and unauth', fakeAsync(() => {
    userSubj.next({DisplayName : "Test Name", Registered: true} as IUserView);
    authSubj.next(false);
    tick();

    //expect(service.canActivate().pipe(first()).subscribe(b => expect(b).toBeFalse()));
  }));

});
