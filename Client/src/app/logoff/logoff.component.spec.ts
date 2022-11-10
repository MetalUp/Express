import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthService, User } from '@auth0/auth0-angular';
import { UserRepresentation } from '@nakedobjects/restful-objects';
import { ConfigService, ContextService } from '@nakedobjects/services';

import { LogoffComponent } from './logoff.component';

describe('LogoffComponent', () => {
  let component: LogoffComponent;
  let fixture: ComponentFixture<LogoffComponent>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let configServiceSpy: jasmine.SpyObj<ConfigService>;
  let locationSpy: jasmine.SpyObj<Location>;

  contextServiceSpy = jasmine.createSpyObj('ContextService', ['getUser']);
  authServiceSpy = jasmine.createSpyObj('AuthService', ['navigate']);
  configServiceSpy = jasmine.createSpyObj('ConfigService', [], { config: { applicationName: "" } });
  locationSpy = jasmine.createSpyObj('Location', ['navigate']);

  contextServiceSpy.getUser.and.returnValue(Promise.resolve({ userName: () => "" } as UserRepresentation));

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LogoffComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: ContextService,
          useValue: contextServiceSpy
        },
        {
          provide: AuthService,
          useValue: authServiceSpy
        },
        {
          provide: ConfigService,
          useValue: configServiceSpy
        },
        {
          provide: Location,
          useValue: locationSpy
        },
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(LogoffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});