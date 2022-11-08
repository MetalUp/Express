import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { TimePickerComponent } from '@nakedobjects/gemini';
import { UserRepresentation } from '@nakedobjects/restful-objects';
import { AuthService, ContextService } from '@nakedobjects/services';

import { RegisteredService } from './registered.service';

describe('RegisteredService', () => {
  let service: RegisteredService;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['canActivate'], {});
    contextServiceSpy = jasmine.createSpyObj('ContextService', ['getUser'], {});

    TestBed.configureTestingModule({});
    service = new RegisteredService(authServiceSpy, contextServiceSpy);

    authServiceSpy.canActivate.and.returnValue(true);
    contextServiceSpy.getUser.and.returnValue(Promise.resolve({ userName: () => "name"} as UserRepresentation));
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should handle canActivate', fakeAsync(() => {
    service.canActivate().then(b => expect(b).toBeTrue());
    tick();
    expect(service.registered).toBeTrue();

  }));

});
