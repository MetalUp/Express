import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '@auth0/auth0-angular';
import { first, Observable, of, pipe, Subject } from 'rxjs';

import { AuthGuard } from './auth.guard';

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let isAuthSubject: Subject<boolean>;

  beforeEach(() => {
    isAuthSubject = new Subject<boolean>();
    authServiceSpy = jasmine.createSpyObj('AuthService', ['load'], { isAuthenticated$: isAuthSubject });
  
    TestBed.configureTestingModule({});
    guard = new AuthGuard(authServiceSpy);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });

  it('should be activated', () => {
 
    const canActivate = guard.canActivate({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot) as Observable<boolean>;
    canActivate.pipe(first()).subscribe(b => expect(b).toBeTrue());
    isAuthSubject.next(true);
  });

  it('should be unactivated', () => {
   
    const canActivate = guard.canActivate({} as ActivatedRouteSnapshot, {} as RouterStateSnapshot) as Observable<boolean>;
    canActivate.pipe(first()).subscribe(b => expect(b).toBeFalse());
    isAuthSubject.next(false);
  });
});
