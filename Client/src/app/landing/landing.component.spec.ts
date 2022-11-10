import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { RegisteredService } from '../services/registered.service';
import { LandingComponent } from './landing.component';

describe('LandingComponent', () => {
  let component: LandingComponent;
  let fixture: ComponentFixture<LandingComponent>;
  let registeredServiceSpy: jasmine.SpyObj<RegisteredService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let registeredSub = new Subject<boolean>();
 
  registeredServiceSpy = jasmine.createSpyObj('RegisteredService', ['isLoggedOn'], { registered$ : registeredSub });
  routerSpy = jasmine.createSpyObj('Router', ['navigate']);

  
  beforeEach(async () => {
   
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

  // it('should navigate home if registered', () => {
   
  //   registeredSub.next(true);

  //   expect(routerSpy.navigate).toHaveBeenCalledWith([`/dashboard`]);
  //   expect(component.userChecked).toBeTrue();
  // });

  // it('should not navigate home if not registered', () => {
    
  //   registeredSub.next(false);

  //   expect(routerSpy.navigate).not.toHaveBeenCalled();
  //   expect(component.userChecked).toBeTrue();
  // });
});
