import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { RegistrationService } from '../services/registration.service';

import { InvitationComponent } from './invitation.component';

describe('InvitationComponent', () => {
  let component: InvitationComponent;
  let fixture: ComponentFixture<InvitationComponent>;

  let registeredServiceSpy: jasmine.SpyObj<RegistrationService>;
  const mapSub = new Subject<ParamMap>();

  const activatedRouteSpy: jasmine.SpyObj<ActivatedRoute> = jasmine.createSpyObj('ActivatedRoute', ['navigate'], { paramMap: mapSub });

  beforeEach(async () => {
    registeredServiceSpy = jasmine.createSpyObj('RegisteredService', ['logout']);
   
    await TestBed.configureTestingModule({
      declarations: [InvitationComponent],
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

  it('should logout if invitation code', () => {
    mapSub.next({ get: () => 'code' } as unknown as ParamMap);

    expect(registeredServiceSpy.logout).toHaveBeenCalledWith('invitation');
    expect(component.showPage).toBeFalse();

    expect(localStorage.getItem("invitationCode")).toBe('code');
    localStorage.removeItem("invitationCode");
  });

  it('should show page if no invitation code', () => {
    mapSub.next({ get: () => null } as unknown as ParamMap);

    expect(registeredServiceSpy.logout).not.toHaveBeenCalled();
    expect(component.showPage).toBeTrue();
  });
});
