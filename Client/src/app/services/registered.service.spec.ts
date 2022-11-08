import { TestBed } from '@angular/core/testing';
import { AuthService, ContextService } from '@nakedobjects/services';

import { RegisteredService } from './registered.service';

describe('RegisteredService', () => {
  let service: RegisteredService;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['load'], {});
    contextServiceSpy = jasmine.createSpyObj('ContextService', ['load'], {});

    TestBed.configureTestingModule({});
    service = new RegisteredService(authServiceSpy, contextServiceSpy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
