import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Subject } from 'rxjs';
import { RegistrationService } from '../services/registration.service';
import { LandingComponent } from './landing.component';

describe('LandingComponent', () => {
  let component: LandingComponent;
  let fixture: ComponentFixture<LandingComponent>;
  let registeredServiceSpy: jasmine.SpyObj<RegistrationService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let registeredSub = new Subject<boolean | undefined>();
  let loggedOnSub = new Subject<boolean>();
  let activatedRouteSpy: jasmine.SpyObj<ActivatedRoute>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;
 
  registeredServiceSpy = jasmine.createSpyObj('RegisteredService', ['isLoggedOn'], { registered$ : registeredSub });
  routerSpy = jasmine.createSpyObj('Router', ['navigate']);

  
  beforeEach(async () => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    registeredSub.next(undefined);
    registeredServiceSpy.isLoggedOn.and.returnValue(loggedOnSub)
   
    await TestBed.configureTestingModule({
      declarations: [ LandingComponent ],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: RegistrationService,
          useValue: registeredServiceSpy
        },
        {
          provide: Router,
          useValue: routerSpy
        },
        {
          provide: ContextService,
          useValue: contextServiceSpy
        },
        {
          provide: RepLoaderService,
          useValue: repLoaderSpy
        },
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

  // it('should navigate home if registered', fakeAsync(() => {
  //   registeredSub.next(true);
  //   tick();
  //   tick();
  //   expect(component.userChecked).toBeTrue();
  //   expect(routerSpy.navigate).toHaveBeenCalledWith(['/dashboard']);
  // }));

  // it('should not navigate home if not registered', fakeAsync(() => {
   
  //   registeredSub.next(false);
  //   tick();
  //   tick();
  //   expect(component.userChecked).toBeTrue();
  //   expect(routerSpy.navigate).not.toHaveBeenCalled();
  // }));
});
