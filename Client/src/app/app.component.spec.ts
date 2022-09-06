import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { AppComponent } from './app.component';
import { Subject} from 'rxjs'
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { AuthService } from '@auth0/auth0-angular';

describe('AppComponent', () => {
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let isAuthSubject = new Subject<boolean>();

  beforeEach(async () => {

    authServiceSpy = jasmine.createSpyObj('AuthService', ['load'], { isAuthenticated$: isAuthSubject });

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        AppComponent
      ],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: AuthService,
          useValue: authServiceSpy
        }
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should be logged in', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    app.ngOnInit();
    isAuthSubject.next(true);
    expect(app.isLoggedIn).toBeTrue();
    app.ngOnDestroy();
  });

  it('should be logged out', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    app.ngOnInit();
    isAuthSubject.next(false);
    expect(app.isLoggedIn).toBeFalse();
    app.ngOnDestroy();
  });
});
