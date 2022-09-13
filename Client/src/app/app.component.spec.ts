import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { AuthService, ConfigService } from '@nakedobjects/services';
import { AppComponent } from './app.component';

describe('AppComponent', () => {
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let configServiceSpy: jasmine.SpyObj<ConfigService>;


  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['handleAuthentication']);
    routerSpy = jasmine.createSpyObj('Router', [], { url: "/dashboard/home" });
    configServiceSpy = jasmine.createSpyObj('ConfigService', [], { loading: 0 });

    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule
      ],
      declarations: [
        AppComponent
      ],
      providers: [
        {
          provide: AuthService,
          useValue: authServiceSpy
        },
        {
          provide: Router,
          useValue: routerSpy
        },
        {
          provide: ConfigService,
          useValue: configServiceSpy
        }
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it(`should call authenticate`, () => {
    TestBed.createComponent(AppComponent);

    expect(authServiceSpy.handleAuthentication).toHaveBeenCalled();
  });

  it('should indicate dashboard', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app.isDashboard()).toBeTrue();
  });
});
