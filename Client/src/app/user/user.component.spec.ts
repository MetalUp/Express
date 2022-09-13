import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
//import { AuthService, User } from '@auth0/auth0-angular';
import { Subject } from 'rxjs';

import { UserComponent } from './user.component';

describe('UserComponent', () => {
  let component: UserComponent;
  let fixture: ComponentFixture<UserComponent>;

  // let authServiceSpy: jasmine.SpyObj<AuthService>;
  // let userSubject = new Subject<User>();

  beforeEach(async () => {

    // authServiceSpy = jasmine.createSpyObj('AuthService', [], { user$: userSubject });
    // await TestBed.configureTestingModule({
    //   declarations: [ UserComponent ],
    //   schemas: [NO_ERRORS_SCHEMA],
    //   providers: [
    //     // {
    //     //   provide: AuthService,
    //     //   useValue: authServiceSpy
    //     // },
    //   ]
    // })
    // .compileComponents();

    // fixture = TestBed.createComponent(UserComponent);
    // component = fixture.componentInstance;
    // fixture.detectChanges();
  });

  it('should create', () => {
   // expect(component).toBeTruthy();
  });

  // it('should default user', () => {

  //   userSubject.next({email : ''});
  
  //   expect(component.userName).toEqual('Unknown');
  // });

  // it('should display email user', () => {
  //   userSubject.next({email : 'testEmail'});

  //   expect(component.userName).toEqual('testEmail');
  // });
});
