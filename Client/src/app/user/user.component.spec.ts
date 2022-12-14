import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Subject } from 'rxjs';
import { UserView } from '../models/user-view';
import { UserService } from '../services/user.service';
import { UserComponent } from './user.component';

describe('UserComponent', () => {
  let component: UserComponent;
  let fixture: ComponentFixture<UserComponent>;

  let userServiceSpy: jasmine.SpyObj<UserService>;

  const userSubj = new Subject<UserView>();

  beforeEach(async () => {

    userServiceSpy = jasmine.createSpyObj('UserService', ['getUser'], {currentUser: userSubj} );

    await TestBed.configureTestingModule({
      declarations: [ UserComponent ],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: UserService,
          useValue: userServiceSpy
        },
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display user', () => {  

    userSubj.next({DisplayName: "Test Name"});


    expect(component.userName).toEqual('Test Name');
  });

});
