import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { AuthService } from '@auth0/auth0-angular';
import { UserRepresentation } from '@nakedobjects/restful-objects';
import { ContextService } from '@nakedobjects/services';
import { Subject } from 'rxjs';

import { RegisteredService } from './registered.service';

describe('RegisteredService', () => {
  let service: RegisteredService;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let authSubj = new Subject<boolean>();


  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['loginWithRedirect'], {isAuthenticated$: authSubj});
    contextServiceSpy = jasmine.createSpyObj('ContextService', ['getUser'], {});

    TestBed.configureTestingModule({});
    service = new RegisteredService(authServiceSpy, contextServiceSpy);

    contextServiceSpy.getUser.and.returnValue(Promise.resolve({ userName: () => "name"} as UserRepresentation));
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should register', fakeAsync(() => {
    authSubj.next(true);
    tick();

    expect(contextServiceSpy.getUser).toHaveBeenCalled();
    expect(service.registered).toBeTrue();

  }));

});
