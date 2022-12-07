import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Subject } from 'rxjs';
import { RegistrationService } from '../services/registration.service';

import { InvitationComponent } from './invitation.component';

describe('InvitationComponent', () => {
  let component: InvitationComponent;
  let fixture: ComponentFixture<InvitationComponent>;

  let registeredServiceSpy: jasmine.SpyObj<RegistrationService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let activatedRouteSpy: jasmine.SpyObj<ActivatedRoute>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;

  let registeredSub = new Subject<boolean | undefined>();
  let loggedOnSub = new Subject<boolean>();
  let mapSub = new Subject<ParamMap>();

  registeredServiceSpy = jasmine.createSpyObj('RegisteredService', ['isLoggedOn'], { registered$ : registeredSub });
  routerSpy = jasmine.createSpyObj('Router', ['navigate']);
  activatedRouteSpy = jasmine.createSpyObj('ActivatedRoute', ['navigate'], {paramMap: mapSub });
  contextServiceSpy = jasmine.createSpyObj('ContextService', ['navigate']);
  repLoaderSpy = jasmine.createSpyObj('RepLoaderService', ['navigate']);


  beforeEach(async () => {

    registeredServiceSpy.isLoggedOn.and.returnValue(loggedOnSub)

    await TestBed.configureTestingModule({
      declarations: [ InvitationComponent ],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: activatedRouteSpy
        },
       
        {
          provide: RegistrationService,
          useValue: registeredServiceSpy
        },
      
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InvitationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
