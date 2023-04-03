import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthService } from '@auth0/auth0-angular';
import { ConfigService } from '@nakedobjects/services';
import { of, Subject } from 'rxjs';
import { UserView } from '../models/user-view';
import { RegistrationService } from '../services/registration.service';
import { UserService } from '../services/user.service';

import { LogoffComponent } from './logoff.component';

describe('LogoffComponent', () => {
  let component: LogoffComponent;
  let fixture: ComponentFixture<LogoffComponent>;
  let userServiceSpy: jasmine.SpyObj<UserService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let registeredServiceSpy: jasmine.SpyObj<RegistrationService>;
  let configServiceSpy: jasmine.SpyObj<ConfigService>;
  let locationSpy: jasmine.SpyObj<Location>;

  const userSubj = new Subject<UserView>();

  userServiceSpy = jasmine.createSpyObj('UserService', ['getUser'], {currentUser: userSubj});
  authServiceSpy = jasmine.createSpyObj('AuthService', [], {isAuthenticated$ : of(false)});
  registeredServiceSpy = jasmine.createSpyObj('RegisteredService', [], {auth: authServiceSpy});
  configServiceSpy = jasmine.createSpyObj('ConfigService', [], { config: { applicationName: "" } });
  locationSpy = jasmine.createSpyObj('Location', ['navigate']);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LogoffComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: UserService,
          useValue: userServiceSpy
        },
        {
          provide: RegistrationService,
          useValue: registeredServiceSpy
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

  it('should get the user', () => {
    userSubj.next({DisplayName: 'Test Name'});


    expect(component.user.DisplayName).toBe('Test Name');
  });
});