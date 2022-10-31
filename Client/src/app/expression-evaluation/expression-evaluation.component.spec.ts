import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ExpressionEvaluationComponent } from './expression-evaluation.component';
import { of, Subject } from 'rxjs';
import { EmptyRunResult, RunResult } from '../models/run-result';
import { wrapExpression } from '../language-helpers/language-helpers';
import { RulesService } from '../services/rules.service';
import { Applicability } from '../models/rules';
import { TaskService } from '../services/task.service';
import { ITask } from '../models/task';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { CompileServerService } from '../services/compile-server.service';

describe('ExpressionEvaluationComponent', () => {
  let component: ExpressionEvaluationComponent;
  let fixture: ComponentFixture<ExpressionEvaluationComponent>;
  let compileServerServiceSpy: jasmine.SpyObj<CompileServerService>;
  let rulesServiceSpy: jasmine.SpyObj<RulesService>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();

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
    compileServerServiceSpy = jasmine.createSpyObj('CompileServerService', ['submit_run'], { "selectedLanguage": "csharp" });
    
    rulesServiceSpy = jasmine.createSpyObj('RulesService', ['filter', 'checkRules']);
    rulesServiceSpy.checkRules.and.returnValue('');
    rulesServiceSpy.filter.and.callFake((_l, _e, tf) => tf);

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
    expect(component.previousExpressionResult).toEqual('');

    component.previousExpressions = [['e1', 'r1'], ['e2', 'r2'], ['e3', 'r3']];
    component.previousExpressionIndex = component.previousExpression.length;

    expect(component.previousExpression).toEqual("e3");
    expect(component.previousExpressionResult).toEqual("r3");

  });

  // it('should show error if no result', () => {

  //   expect(component.previousExpression).toEqual('');
  //   expect(component.previousExpressionResult).toEqual('');

  //   component.result = testRunResultErr;

  //   expect(component.previousExpression).toEqual('');
  //   expect(component.previousExpressionResult).toEqual('run error');

  //   component.result = testRunResultCmp;

  //   expect(component.previousExpression).toEqual('');
  //   expect(component.previousExpressionResult).toEqual('');

  //   component.validationFail = "validFail"

  //   expect(component.previousExpression).toEqual('');
  //   expect(component.previousExpressionResult).toEqual('');

  //   component.result = EmptyRunResult;
  // });

  // it('should submit code on enter and show result', () => {
  //   compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOK));

  //   component.expression = 'test';
  //   const wrapped = wrapExpression(component.selectedLanguage, component.expression);

  //   component.onEnter();
  //   expect(compileServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped, true);

  //   expect(component.expression).toBe('test');
  //   expect(component.previousExpression).toBe('test');
  //   expect(component.previousExpressionResult).toBe('expression result');
  //   expect(component.expressionError).toBe('');

  // });

  // it('should submit code on enter and show result without trimming', () => {
  //   compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOKWhiteSpace));

  //   component.expression = 'test';
  //   const wrapped = wrapExpression(component.selectedLanguage, component.expression);

  //   component.onEnter();
  //   expect(compileServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped, true);

  //   expect(component.expression).toBe('test');
  //   expect(component.previousExpression).toBe('test');
  //   expect(component.previousExpressionResult).toBe(' expression result');
  //   expect(component.expressionError).toBe('');
  // });


  // it('should submit code on enter and show compiler error', () => {
  //   compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultCmp));

  //   component.expression = 'test';
  //   const wrapped = wrapExpression(component.selectedLanguage, component.expression);

  //   component.onEnter();
  //   expect(compileServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped, true);

  //   expect(component.expression).toBe('test');
  //   expect(component.previousExpression).toBe('test');
  //   expect(component.previousExpressionResult).toBe('');
  //   expect(component.expressionError).toBe('compiler error');
  // });

  // it('should submit code on enter and show runtime error', () => {
  //   compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultErr));

  //   component.expression = 'test';
  //   const wrapped = wrapExpression(component.selectedLanguage, component.expression);

  //   component.onEnter();
  //   expect(compileServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped, true);

  //   expect(component.expression).toBe('test');
  //   expect(component.previousExpression).toBe('test');
  //   expect(component.previousExpressionResult).toBe('run error');
  //   expect(component.expressionError).toBe('');
  // });

  // it('should ignore empty expressions', () => {

  //   compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOK));

  //   component.expression = '';
  //   component.onEnter();
  //   expect(compileServerServiceSpy.submit_run).not.toHaveBeenCalled();
  //   expect(component.previousExpressionResult).toBe('');
  //   expect(component.expressionError).toBe('');
  // });

  // it('should call checkRules on enter', () => {

  //   compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOK));

  //   component.expression = 'test';
  //   const wrapped = wrapExpression(component.selectedLanguage, component.expression);

  //   component.onEnter();
  //   expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith("csharp", Applicability.expressions, "test");
  //   expect(compileServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped, true);

  //   expect(component.expression).toBe('test');
  //   expect(component.previousExpression).toBe('test');
  // });

  // it('should not submit on checkRules error', () => {

  //   compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOK));
  //   rulesServiceSpy.checkRules.and.returnValue("rules fail");

  //   component.expression = 'test';

  //   component.onEnter();
  //   expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith("csharp", Applicability.expressions, "test");
  //   expect(compileServerServiceSpy.submit_run).not.toHaveBeenCalled();

  //   expect(component.expression).toBe('test');
  //   expect(component.previousExpression).toBe('test');
  //   expect(component.previousExpressionResult).toBe('');
  //   expect(component.expressionError).toBe('rules fail');
  // });

  // it('should disable paste by default', () => {

  //   let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);

  //   component.onPaste(eventSpy);
  //   expect(eventSpy.preventDefault).toHaveBeenCalled();
  // });

  // it('should enable paste from task', () => {

  //   let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
  //   taskSubject.next({ PasteExpression: true } as ITask);

  //   component.onPaste(eventSpy);
  //   expect(eventSpy.preventDefault).not.toHaveBeenCalled();
  // });

  // it('should disable paste from task', () => {

  //   let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
  //   taskSubject.next({ PasteExpression: false } as ITask);

  //   component.onPaste(eventSpy);
  //   expect(eventSpy.preventDefault).toHaveBeenCalled();
  // });
});
