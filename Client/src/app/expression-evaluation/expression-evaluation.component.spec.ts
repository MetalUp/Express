import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ExpressionEvaluationComponent } from './expression-evaluation.component';
import { of, Subject } from 'rxjs';
import { EmptyRunResult, RunResult } from '../models/run-result';
import { RulesService } from '../services/rules.service';
import { Applicability } from '../models/rules';
import { TaskService } from '../services/task.service';
import { ITaskUserView } from '../models/task-user-view';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { CompileServerService } from '../services/compile-server.service';

describe('ExpressionEvaluationComponent', () => {
  let component: ExpressionEvaluationComponent;
  let fixture: ComponentFixture<ExpressionEvaluationComponent>;
  let compileServerServiceSpy: jasmine.SpyObj<CompileServerService>;
  let rulesServiceSpy: jasmine.SpyObj<RulesService>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITaskUserView>();
  let resultSubject = new Subject<RunResult>();

  let testRunResultOK: RunResult = {
    run_id: 'a',
    outcome: 15,
    cmpinfo: '',
    stdout: 'expression result',
    stderr: ''
  }

  let testRunResultOKWhiteSpace: RunResult = {
    run_id: 'a',
    outcome: 15,
    cmpinfo: '',
    stdout: ' expression result',
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
    compileServerServiceSpy = jasmine.createSpyObj('CompileServerService', ['evaluateExpression'], { "selectedLanguage": "csharp", lastExpressionResult: resultSubject });
    
    rulesServiceSpy = jasmine.createSpyObj('RulesService', ['filter', 'checkRules']);
    rulesServiceSpy.checkRules.and.returnValue('');
    rulesServiceSpy.filter.and.callFake(( _e, tf) => tf);

    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], { currentTask: taskSubject });

    await TestBed.configureTestingModule({
      declarations: [ExpressionEvaluationComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: CompileServerService,
          useValue: compileServerServiceSpy
        },
        {
          provide: RulesService,
          useValue: rulesServiceSpy
        },
        {
          provide: TaskService,
          useValue: taskServiceSpy
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

    component.onKey(<any>{ key: 'ArrowUp' });
    expect(component.expression).toEqual("e2");
    component.onKey(<any>{ key: 'ArrowUp' });
    expect(component.expression).toEqual("e1");
    component.onKey(<any>{ key: 'ArrowUp' });
    expect(component.expression).toEqual("e1");
    component.onKey(<any>{ key: 'ArrowDown' });
    expect(component.expression).toEqual("e2");
    component.onKey(<any>{ key: 'ArrowDown' });
    expect(component.expression).toEqual("e3");
    component.onKey(<any>{ key: 'ArrowDown' });
    expect(component.expression).toEqual('');
    component.onKey(<any>{ key: 'ArrowDown' });
    expect(component.expression).toEqual('');
    component.onKey(<any>{ key: 'ArrowUp' });
    expect(component.expression).toEqual("e3");
  });

  it('should show the most recent previous expression', () => {

    expect(component.previousExpression).toEqual('');
    

    component.previousExpressions = [['e1', 'r1'], ['e2', 'r2'], ['e3', 'r3']];
    component.previousExpressionIndex = component.previousExpression.length;

    expect(component.previousExpression).toEqual("e3");
    

  });

  it('should show error if no result', () => {

    expect(component.previousExpression).toEqual('');
   

    component.result = testRunResultErr;

    expect(component.previousExpression).toEqual('');
    

    component.result = testRunResultCmp;

    expect(component.previousExpression).toEqual('');
   

    component.validationFail = "validFail"

    expect(component.previousExpression).toEqual('');
   

    component.result = EmptyRunResult;
  });

  it('should submit code on enter and show result', () => {
    compileServerServiceSpy.evaluateExpression.and.returnValue(of<RunResult>(testRunResultOK));

    component.taskId = 66;
    component.expression = 'test';

    component.onSubmit();
    expect(compileServerServiceSpy.evaluateExpression).toHaveBeenCalledWith(66, "test");

    expect(component.expression).toBe('test');
    expect(component.previousExpression).toBe('test');
   
    expect(component.expressionError).toBe('');

  });

  it('should submit code on enter and show result without trimming', () => {
    compileServerServiceSpy.evaluateExpression.and.returnValue(of<RunResult>(testRunResultOKWhiteSpace));

    component.taskId = 66;
    component.expression = 'test';
   
    component.onSubmit();
    expect(compileServerServiceSpy.evaluateExpression).toHaveBeenCalledWith(66, "test");

    expect(component.expression).toBe('test');
    expect(component.previousExpression).toBe('test');
    
    expect(component.expressionError).toBe('');
  });


  it('should submit code on enter and show compiler error', () => {
    compileServerServiceSpy.evaluateExpression.and.returnValue(of<RunResult>(testRunResultCmp));

    component.taskId = 66;
    component.expression = 'test';
  
    component.onSubmit();
    expect(compileServerServiceSpy.evaluateExpression).toHaveBeenCalledWith(66, "test");

    expect(component.expression).toBe('test');
    expect(component.previousExpression).toBe('test');
   
    expect(component.expressionError).toBe('compiler error');
  });

  it('should submit code on enter and show runtime error', () => {
    compileServerServiceSpy.evaluateExpression.and.returnValue(of<RunResult>(testRunResultErr));

    component.taskId = 66;
    component.expression = 'test';
   
    component.onSubmit();
    expect(compileServerServiceSpy.evaluateExpression).toHaveBeenCalledWith(66, "test");

    expect(component.expression).toBe('test');
    expect(component.previousExpression).toBe('test');
    
    expect(component.expressionError).toBe('');
  });

  it('should ignore empty expressions', () => {

    compileServerServiceSpy.evaluateExpression.and.returnValue(of<RunResult>(testRunResultOK));

    component.taskId = 66;
    component.expression = '';
    component.onSubmit();
    expect(compileServerServiceSpy.evaluateExpression).not.toHaveBeenCalled();
   
    expect(component.expressionError).toBe('');
  });

  it('should call checkRules on enter', () => {

    compileServerServiceSpy.evaluateExpression.and.returnValue(of<RunResult>(testRunResultOK));

    component.taskId = 66;
    component.expression = 'test';
   
    component.onSubmit();
    expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith(Applicability.expressions, "test");
    expect(compileServerServiceSpy.evaluateExpression).toHaveBeenCalledWith(66, "test");

    expect(component.expression).toBe('test');
    expect(component.previousExpression).toBe('test');
  });

  it('should not submit on checkRules error', () => {

    compileServerServiceSpy.evaluateExpression.and.returnValue(of<RunResult>(testRunResultOK));
    rulesServiceSpy.checkRules.and.returnValue("rules fail");

    component.taskId = 66;
    component.expression = 'test';

    component.onSubmit();
    expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith( Applicability.expressions, "test");
    expect(compileServerServiceSpy.evaluateExpression).not.toHaveBeenCalled();

    expect(component.expression).toBe('test');
    expect(component.previousExpression).toBe('test');
    
    expect(component.expressionError).toBe('rules fail');
  });

  it('should disable paste by default', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });

  it('should enable paste from task', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteExpression: true } as ITaskUserView);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).not.toHaveBeenCalled();
  });

  it('should disable paste from task', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteExpression: false } as ITaskUserView);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });
});
