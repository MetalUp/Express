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
  
  const userSubj = new Subject<UserView>();

  const userServiceSpy: jasmine.SpyObj<UserService> = jasmine.createSpyObj('UserService', ['getUser'], {currentUser: userSubj});
  const authServiceSpy: jasmine.SpyObj<AuthService> = jasmine.createSpyObj('AuthService', [], {isAuthenticated$ : of(false)});
  const registeredServiceSpy: jasmine.SpyObj<RegistrationService> = jasmine.createSpyObj('RegisteredService', [], {auth: authServiceSpy});
  const configServiceSpy: jasmine.SpyObj<ConfigService> = jasmine.createSpyObj('ConfigService', [], { config: { applicationName: "" } });
  const locationSpy: jasmine.SpyObj<Location> = jasmine.createSpyObj('Location', ['navigate']);

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