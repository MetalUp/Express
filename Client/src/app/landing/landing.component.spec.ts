import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { UserRepresentation } from '@nakedobjects/restful-objects';
import { ContextService } from '@nakedobjects/services';

import { LandingComponent } from './landing.component';

describe('LandingComponent', () => {
  let component: LandingComponent;
  let fixture: ComponentFixture<LandingComponent>;


  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let testUser = {
    userName: () => 'testName'
  }

  contextServiceSpy = jasmine.createSpyObj('ContextService', ['getUser']);
  routerSpy = jasmine.createSpyObj('Router', ['navigate']);

  contextServiceSpy.getUser.and.returnValue(Promise.resolve(testUser as UserRepresentation));

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LandingComponent ],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: ContextService,
          useValue: contextServiceSpy
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