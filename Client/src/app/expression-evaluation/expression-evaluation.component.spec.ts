import { ComponentFixture, TestBed } from '@angular/core/testing';
import { JobeServerService } from '../services/jobe-server.service';
import { ExpressionEvaluationComponent } from './expression-evaluation.component';
import { of, Subject } from 'rxjs';
import { EmptyRunResult, RunResult } from '../services/run-result';
import { wrapExpression } from '../languages/language-helpers';
import { RulesService } from '../services/rules.service';
import { Applicability } from '../services/rules';
import { TaskService } from '../services/task.service';
import { ITask } from '../services/task';

describe('ExpressionEvaluationComponent', () => {
  let component: ExpressionEvaluationComponent;
  let fixture: ComponentFixture<ExpressionEvaluationComponent>;
  let jobeServerServiceSpy: jasmine.SpyObj<JobeServerService>;
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
    jobeServerServiceSpy = jasmine.createSpyObj('JobeServerService', ['submit_run', 'get_languages'], { "selectedLanguage": "csharp" });
    jobeServerServiceSpy.get_languages.and.returnValue(of<[string, string][]>([["1", "2"]]));

    rulesServiceSpy = jasmine.createSpyObj('RulesService', ['filter', 'checkRules']);
    rulesServiceSpy.checkRules.and.returnValue('');
    rulesServiceSpy.filter.and.callFake((_l, _e, tf) => tf);

    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], { currentTask: taskSubject });

    await TestBed.configureTestingModule({
      declarations: [ExpressionEvaluationComponent],
      providers: [
        {
          provide: JobeServerService,
          useValue: jobeServerServiceSpy
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

  it('should show error if no result', () => {

    expect(component.previousExpression).toEqual('');
    expect(component.previousExpressionResult).toEqual('');

    component.result = testRunResultErr;

    expect(component.previousExpression).toEqual('');
    expect(component.previousExpressionResult).toEqual('run error');

    component.result = testRunResultCmp;

    expect(component.previousExpression).toEqual('');
    expect(component.previousExpressionResult).toEqual('compiler error');

    component.validationFail = "validFail"

    expect(component.previousExpression).toEqual('');
    expect(component.previousExpressionResult).toEqual('validFail');

    component.result = EmptyRunResult;
  });

  it('should submit code on enter and show result', () => {
    jobeServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOK));

    component.expression = 'test';
    const wrapped = wrapExpression(component.selectedLanguage, component.expression);

    component.onEnter();
    expect(jobeServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.expression).toBe('');
    expect(component.previousExpression).toBe('test');
    expect(component.previousExpressionResult).toBe('expression result')

  });

  it('should submit code on enter and show compiler error', () => {
    jobeServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultCmp));

    component.expression = 'test';
    const wrapped = wrapExpression(component.selectedLanguage, component.expression);

    component.onEnter();
    expect(jobeServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.expression).toBe('');
    expect(component.previousExpression).toBe('test');
    expect(component.previousExpressionResult).toBe('compiler error')
  });

  it('should submit code on enter and show runtime error', () => {
    jobeServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultErr));

    component.expression = 'test';
    const wrapped = wrapExpression(component.selectedLanguage, component.expression);

    component.onEnter();
    expect(jobeServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.expression).toBe('');
    expect(component.previousExpression).toBe('test');
    expect(component.previousExpressionResult).toBe('run error')
  });

  it('should ignore empty expressions', () => {

    jobeServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOK));

    component.expression = '';
    component.onEnter();
    expect(jobeServerServiceSpy.submit_run).not.toHaveBeenCalled();
    expect(component.previousExpressionResult).toBe('');
  });

  it('should call checkRules on enter', () => {

    jobeServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOK));

    component.expression = 'test';
    const wrapped = wrapExpression(component.selectedLanguage, component.expression);

    component.onEnter();
    expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith("csharp", Applicability.expressions, "test");
    expect(jobeServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.expression).toBe('');
    expect(component.previousExpression).toBe('test');
  });

  it('should not submit on checkRules error', () => {

    jobeServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOK));
    rulesServiceSpy.checkRules.and.returnValue("rules fail");

    component.expression = 'test';

    component.onEnter();
    expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith("csharp", Applicability.expressions, "test");
    expect(jobeServerServiceSpy.submit_run).not.toHaveBeenCalled();

    expect(component.expression).toBe('');
    expect(component.previousExpression).toBe('test');
    expect(component.previousExpressionResult).toBe('rules fail')
  });

  it('should disable paste by default', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });

  it('should enable paste from task', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteExpression: true } as ITask);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).not.toHaveBeenCalled();
  });

  it('should disable paste from task', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteExpression: false } as ITask);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });
});
