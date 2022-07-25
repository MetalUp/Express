import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Params } from '@angular/router';
import { JobeServerService } from '../services/jobe-server.service';
import { ExpressionEvaluationComponent } from './expression-evaluation.component';
import { of } from 'rxjs';

describe('ExpressionEvaluationComponent', () => {
  let component: ExpressionEvaluationComponent;
  let fixture: ComponentFixture<ExpressionEvaluationComponent>;
  let mockJobeServerService: jasmine.SpyObj<JobeServerService>;

  let testParams : any  = {};

  beforeEach(async () => {
    mockJobeServerService = jasmine.createSpyObj('JobeServerService', ['submit_run', 'get_languages']);
    mockJobeServerService.get_languages.and.returnValue(of<[string, string][]>([["1", "2"]]));

    await TestBed.configureTestingModule({
      declarations: [ExpressionEvaluationComponent],
      providers: [
        {
          provide: JobeServerService,
          useValue: mockJobeServerService
        },
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of<Params>(testParams)
          },
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ExpressionEvaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should setup selected language', () => {
    mockJobeServerService.get_languages.and.returnValue(of<[string, string][]>([["language1", "language2"]]));
    testParams['language'] =  'language1';
   
    component.ngOnInit();

    expect(component.selectedLanguage).toEqual("language1");
  });


  it('should retrieve previous expressions', () => {
    component.previousExpressions = [['e1', 'r1'], ['e2', 'r2'], ['e3', 'r3']];
    component.previousExpressionIndex = component.previousExpression.length;

    component.onUp();
    expect(component.expression).toEqual("e2");
    component.onUp();
    expect(component.expression).toEqual("e1");
    component.onUp();
    expect(component.expression).toEqual("e1");
    component.onDown();
    expect(component.expression).toEqual("e2");
    component.onDown();
    expect(component.expression).toEqual("e3");
    component.onDown();
    expect(component.expression).toEqual("");
    component.onDown();
    expect(component.expression).toEqual("");
    component.onUp();
    expect(component.expression).toEqual("e3");
  });



});
