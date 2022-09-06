import { TestBed } from '@angular/core/testing';
import { AuthService } from '@auth0/auth0-angular';
import { of } from 'rxjs';

import { AuthGuard } from './auth.guard';

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['load'], { isAuthenticated$: of(true) });

    TestBed.configureTestingModule({});
    guard = new AuthGuard(authServiceSpy);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
