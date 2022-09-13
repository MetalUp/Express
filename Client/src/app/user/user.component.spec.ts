import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { UserRepresentation } from '@nakedobjects/restful-objects';
import { ContextService } from '@nakedobjects/services';
//import { AuthService, User } from '@auth0/auth0-angular';
import { Subject } from 'rxjs';

import { UserComponent } from './user.component';

describe('UserComponent', () => {
  let component: UserComponent;
  let fixture: ComponentFixture<UserComponent>;

  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let testUser = {
    userName : () => 'testName'
  }

  beforeEach(async () => {

    contextServiceSpy = jasmine.createSpyObj('ContextService', ['getUser']);

    contextServiceSpy.getUser.and.returnValue(Promise.resolve(testUser as UserRepresentation));

    await TestBed.configureTestingModule({
      declarations: [ UserComponent ],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: ContextService,
          useValue: contextServiceSpy
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
    expect(component.userName).toEqual('testName');
  });

});
