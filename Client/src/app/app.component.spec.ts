import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { AuthService, ConfigService, RepLoaderService } from '@nakedobjects/services';
import { Subject } from 'rxjs';
import { AppComponent } from './app.component';
import { RegisteredService } from './services/registered.service';

describe('AppComponent', () => {
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let registeredServiceSpy: jasmine.SpyObj<RegisteredService>;
  let routerSpy: jasmine.SpyObj<Router>;
  let configServiceSpy: jasmine.SpyObj<ConfigService>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;
  let loadingSubj = new Subject<number>();


  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj('AuthService', ['handleAuthentication']);
    routerSpy = jasmine.createSpyObj('Router', [], { url: "/dashboard/home" });
    configServiceSpy = jasmine.createSpyObj('ConfigService', [], { loading: 0 });
    repLoaderSpy = jasmine.createSpyObj('RepLoaderService', [], { loadingCount$: loadingSubj })
    registeredServiceSpy = jasmine.createSpyObj('RegisteredService', [], {registered: true  })

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
          provide: RegisteredService,
          useValue: registeredServiceSpy
        },
        {
          provide: Router,
          useValue: routerSpy
        },
        {
          provide: RepLoaderService,
          useValue: repLoaderSpy
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

  it('should flag loading', () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    app.ngOnInit();
    expect(app.loading).toBeFalse();
    loadingSubj.next(1);
    expect(app.loading).toBeTrue();
    loadingSubj.next(0);
    expect(app.loading).toBeFalse();
  });
});
