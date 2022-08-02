import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Params } from '@angular/router';
import { of } from 'rxjs';
import { JobeServerService } from '../services/jobe-server.service';
import { SelectedLanguageComponent } from './selected-language.component';

describe('SelectedLanguageComponent', () => {
  let component: SelectedLanguageComponent;
  let fixture: ComponentFixture<SelectedLanguageComponent>;
  let jobeServerServiceSpy: jasmine.SpyObj<JobeServerService>;
  let testParams: any = {};

  beforeEach(async () => {
    jobeServerServiceSpy = jasmine.createSpyObj('JobeServerService', ['submit_run', 'get_languages']);
    jobeServerServiceSpy.get_languages.and.returnValue(of<[string, string][]>([["1", "2"]]));

    await TestBed.configureTestingModule({
      declarations: [SelectedLanguageComponent],
      providers: [
        {
          provide: JobeServerService,
          useValue: jobeServerServiceSpy
        },
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of<Params>(testParams)
          },
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SelectedLanguageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should setup selected language', () => {
    jobeServerServiceSpy.get_languages.and.returnValue(of<[string, string][]>([["language1", "language2"]]));
    testParams['language'] = 'language1';

    component.ngOnInit();

    expect(component.selectedLanguage).toEqual("language1");
  });

});
