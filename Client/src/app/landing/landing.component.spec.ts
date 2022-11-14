import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { RegisteredService } from '../services/registered.service';
import { LandingComponent } from './landing.component';

describe('LandingComponent', () => {
  let component: LandingComponent;
  let fixture: ComponentFixture<LandingComponent>;
  let registeredServiceSpy: jasmine.SpyObj<RegisteredService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let registeredSub = new Subject<boolean | undefined>();
 
  registeredServiceSpy = jasmine.createSpyObj('RegisteredService', ['isLoggedOn'], { registered$ : registeredSub });
  routerSpy = jasmine.createSpyObj('Router', ['navigate']);

  
  beforeEach(async () => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    registeredSub.next(undefined);

   
    await TestBed.configureTestingModule({
      declarations: [ LandingComponent ],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: RegisteredService,
          useValue: registeredServiceSpy
        },
        {
          provide: Router,
          useValue: routerSpy
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

  it('should navigate home if registered', fakeAsync(() => {
    registeredSub.next(true);
    tick();
    tick();
    expect(component.userChecked).toBeTrue();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/dashboard']);
  }));

  it('should not navigate home if not registered', fakeAsync(() => {
   
    registeredSub.next(false);
    tick();
    tick();
    expect(component.userChecked).toBeTrue();
    expect(routerSpy.navigate).not.toHaveBeenCalled();
  }));
});
