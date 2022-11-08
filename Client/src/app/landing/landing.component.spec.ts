import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { UserRepresentation } from '@nakedobjects/restful-objects';
import { ContextService } from '@nakedobjects/services';
import { RegisteredService } from '../services/registered.service';

import { LandingComponent } from './landing.component';

describe('LandingComponent', () => {
  let component: LandingComponent;
  let fixture: ComponentFixture<LandingComponent>;


  let registeredServiceSpy: jasmine.SpyObj<RegisteredService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let testUser = {
    userName: () => 'testName'
  }

  registeredServiceSpy = jasmine.createSpyObj('RegisteredService', ['isRegistered']);
  routerSpy = jasmine.createSpyObj('Router', ['navigate']);

  registeredServiceSpy.isRegistered.and.returnValue(Promise.resolve(true));

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
});
