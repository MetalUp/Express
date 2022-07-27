import { ComponentFixture, TestBed } from '@angular/core/testing';
import { JobeServerService } from '../services/jobe-server.service';
import { ExpressionEvaluationComponent } from './expression-evaluation.component';
import { of } from 'rxjs';
import { EmptyRunResult, RunResult } from '../services/run-result';
import { wrapExpression } from '../languages/language-helpers';
import { RulesService } from '../services/rules.service';

describe('ExpressionEvaluationComponent', () => {
  let component: ExpressionEvaluationComponent;
  let fixture: ComponentFixture<ExpressionEvaluationComponent>;
  let mockJobeServerService: jasmine.SpyObj<JobeServerService>;
  let mockRulesService: jasmine.SpyObj<RulesService>;

  let testParams : any  = {};

  let testRunResultOK: RunResult = {
    run_id: 'a',
    outcome: 15,
    cmpinfo: '',
    stdout: 'expression result',
    stderr: ''
  }

  let testRunResultCmp: RunResult = {
    run_id: 'a',
    outcome: 11,
    cmpinfo: 'compiler error',
    stdout: '',
    stderr: ''
  }

  let testRunResultErr: RunResult = {
    run_id: 'a',
    outcome: 12,
    cmpinfo: '',
    stdout: '',
    stderr: 'run error'
  }


  beforeEach(async () => {
    mockJobeServerService = jasmine.createSpyObj('JobeServerService', ['submit_run', 'get_languages'], {"selectedLanguage":"csharp"});
    mockJobeServerService.get_languages.and.returnValue(of<[string, string][]>([["1", "2"]]));
    mockRulesService = jasmine.createSpyObj('RulesService', ['filter', 'validate', 'parse']);

    mockRulesService.parse.and.returnValue('');
    mockRulesService.validate.and.returnValue('');
    mockRulesService.filter.and.returnValue('');
   
    await TestBed.configureTestingModule({
      declarations: [ExpressionEvaluationComponent],
      providers: [
        {
          provide: JobeServerService,
          useValue: mockJobeServerService
        },
        {
          provide: RulesService,
          useValue: mockRulesService
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

  it('should retrieve previous expressions', () => {
    component.previousExpressions = [['e1', 'r1'], ['e2', 'r2'], ['e3', 'r3']];
    component.previousExpressionIndex = component.previousExpression.length;

    component.onKey(<any>{key :'ArrowUp'});
    expect(component.expression).toEqual("e2");
    component.onKey(<any>{key :'ArrowUp'});
    expect(component.expression).toEqual("e1");
    component.onKey(<any>{key :'ArrowUp'});
    expect(component.expression).toEqual("e1");
    component.onKey(<any>{key :'ArrowDown'});
    expect(component.expression).toEqual("e2");
    component.onKey(<any>{key :'ArrowDown'});
    expect(component.expression).toEqual("e3");
    component.onKey(<any>{key :'ArrowDown'});
    expect(component.expression).toEqual('');
    component.onKey(<any>{key :'ArrowDown'});
    expect(component.expression).toEqual('');
    component.onKey(<any>{key :'ArrowUp'});
    expect(component.expression).toEqual("e3");
  });

  it('should show the most recent previous expression', () => {

    expect(component.previousExpression).toEqual('');
    expect(component.previousExpressionResult).toEqual('');
    
    component.previousExpressions = [['e1', 'r1'], ['e2', 'r2'], ['e3', 'r3']];
    component.previousExpressionIndex = component.previousExpression.length;

    expect(component.previousExpression).toEqual("e3");
    expect(component.previousExpressionResult).toEqual("r3");
    
  });

  it('should show error if no result', () => {
  
    expect(component.previousExpression).toEqual('');
    expect(component.previousExpressionResult).toEqual('');

    component.result = testRunResultErr;

    mockRulesService.filter.and.returnValue('run error');

    expect(component.previousExpression).toEqual('');
    expect(component.previousExpressionResult).toEqual('run error');

    component.result = testRunResultCmp;

    mockRulesService.filter.and.returnValue('compiler error');

    expect(component.previousExpression).toEqual('');
    expect(component.previousExpressionResult).toEqual('compiler error');

    component.validationFail = "validFail"

    expect(component.previousExpression).toEqual('');
    expect(component.previousExpressionResult).toEqual('validFail');
    
    component.result = EmptyRunResult;
  });

  it('should submit code on enter and show result', () => {
    mockJobeServerService.submit_run.and.returnValue(of<RunResult>(testRunResultOK));
    
    component.expression = 'test';
    const wrapped = wrapExpression(component.selectedLanguage, component.expression);

    component.onEnter();
    expect(mockJobeServerService.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.expression).toBe('');
    expect(component.previousExpression).toBe('test');
    expect(component.previousExpressionResult).toBe('expression result')

  });

  it('should submit code on enter and show compiler error', () => {
    mockJobeServerService.submit_run.and.returnValue(of<RunResult>(testRunResultCmp));
    mockRulesService.filter.and.returnValue('compiler error');

    component.expression = 'test';
    const wrapped = wrapExpression(component.selectedLanguage, component.expression);

    component.onEnter();
    expect(mockJobeServerService.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.expression).toBe('');
    expect(component.previousExpression).toBe('test');
    expect(component.previousExpressionResult).toBe('compiler error')

  });

  it('should submit code on enter and show runtime error', () => {
    mockJobeServerService.submit_run.and.returnValue(of<RunResult>(testRunResultErr));
    mockRulesService.filter.and.returnValue('run error');

    component.expression = 'test';
    const wrapped = wrapExpression(component.selectedLanguage, component.expression);

    component.onEnter();
    expect(mockJobeServerService.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.expression).toBe('');
    expect(component.previousExpression).toBe('test');
    expect(component.previousExpressionResult).toBe('run error')

  });


  // it('should ignore code if unrecognised language', () => {
  //   mockRulesService.parse.and.returnValue('');
  //   mockRulesService.validate.and.returnValue('');
  //   mockJobeServerService.submit_run.and.returnValue(of<RunResult>(testRunResultOK));

  //   (Object.getOwnPropertyDescriptor(mockJobeServerService, "selectedLanguage") as any).get.and.returnValue('');

  //   component.expression = 'test';
  //   component.onEnter();
  //   expect(mockJobeServerService.submit_run).not.toHaveBeenCalled();
  //   expect(component.previousExpressionResult).toBe('unknown language');
  // });

  it('should ignore empty expressions', () => {
   
    mockJobeServerService.submit_run.and.returnValue(of<RunResult>(testRunResultOK));

    component.expression = '';
    component.onEnter();
    expect(mockJobeServerService.submit_run).not.toHaveBeenCalled();
    expect(component.previousExpressionResult).toBe('');
  });

});
