import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Subject } from 'rxjs';
import { RegistrationService } from '../services/registration.service';
import { UserService } from '../services/user.service';
import { LandingComponent } from './landing.component';

describe('LandingComponent', () => {
  let component: LandingComponent;
  let fixture: ComponentFixture<LandingComponent>;

  let registeredServiceSpy: jasmine.SpyObj<RegistrationService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
 
  let registeredSub = new Subject<boolean | undefined>();
  let loggedOnSub = new Subject<boolean>();
  
  registeredServiceSpy = jasmine.createSpyObj('RegisteredService', ['isLoggedOn'], { registered$ : registeredSub });
  routerSpy = jasmine.createSpyObj('Router', ['navigate']);
  userServiceSpy = jasmine.createSpyObj('UserService', ['acceptInvitation']);

  
  beforeEach(async () => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    registeredSub.next(undefined);
    registeredServiceSpy.isLoggedOn.and.returnValue(loggedOnSub)
    userServiceSpy.acceptInvitation.and.returnValue(Promise.resolve(true));

    await TestBed.configureTestingModule({
      declarations: [ LandingComponent ],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: Router,
          useValue: routerSpy
        },
        {
          provide: RegistrationService,
          useValue: registeredServiceSpy
        },
        {
          provide: UserService,
          useValue: userServiceSpy
        }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LandingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate home if registered', fakeAsync(() => {
    registeredSub.next(true);
    tick();
    tick();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/dashboard']);
  }));

  it('should not navigate home if not registered', fakeAsync(() => {
   
    registeredSub.next(false);
    tick();
    tick();
    expect(component.userChecked).toBeTrue();
    expect(component.pendingStatus).toBeFalse();
    expect(routerSpy.navigate).not.toHaveBeenCalled();
  }));

  it('should accept invitation if logged on and code set', fakeAsync(() => {
    localStorage.setItem(RegistrationService.inviteCodeKey, "testcode");

    loggedOnSub.next(true);
    tick();
    tick();
    expect(component.pendingStatus).toBeTrue();
    expect(userServiceSpy.acceptInvitation).toHaveBeenCalledWith('testcode');
  }));
});
