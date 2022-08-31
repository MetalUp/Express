import { ComponentFixture, TestBed } from '@angular/core/testing';
import { wrapFunctions } from '../languages/language-helpers';
import { JobeServerService } from '../services/jobe-server.service';
import { RunResult } from '../services/run-result';
import { of, Subject } from 'rxjs';

import { FunctionDefinitionComponent } from './function-definition.component';
import { RulesService } from '../services/rules.service';
import { Applicability } from '../services/rules';
import { TaskService } from '../services/task.service';
import { ITask } from '../services/task';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('FunctionDefinitionComponent', () => {
  let component: FunctionDefinitionComponent;
  let fixture: ComponentFixture<FunctionDefinitionComponent>;
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
    jobeServerServiceSpy = jasmine.createSpyObj('JobeServerService', ['submit_run', 'clearFunctionDefinitions', 'setFunctionDefinitions'], { "selectedLanguage": "csharp" });

    rulesServiceSpy = jasmine.createSpyObj('RulesService', ['filter', 'checkRules']);
    rulesServiceSpy.checkRules.and.returnValue('');
    rulesServiceSpy.filter.and.callFake((_l, _e, tf) => tf);

    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], { currentTask: taskSubject });

    await TestBed.configureTestingModule({
      declarations: [FunctionDefinitionComponent],
      schemas: [NO_ERRORS_SCHEMA],
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
    })
      .compileComponents();

    fixture = TestBed.createComponent(FunctionDefinitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });


  it('should submit code for compile OK', () => {
    jobeServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOK));

    component.functionDefinitions = 'test';
    const wrapped = wrapFunctions('csharp', component.functionDefinitions);

    component.onSubmit();
    expect(jobeServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.compiledOK).toBe(true);
    expect(component.currentStatus).toBe('Compiled OK');
    expect(component.pendingSubmit).toBe(false);

    expect(jobeServerServiceSpy.setFunctionDefinitions).toHaveBeenCalledWith(component.functionDefinitions);

  });

  it('should submit code for compile Fail', () => {
    jobeServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultCmp));

    component.functionDefinitions = 'test';
    const wrapped = wrapFunctions('csharp', component.functionDefinitions);

    component.onSubmit();
    expect(jobeServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('compiler error');
    expect(component.pendingSubmit).toBe(false);

    expect(jobeServerServiceSpy.setFunctionDefinitions).not.toHaveBeenCalledWith(component.functionDefinitions);

  });

  it('should submit code for compile Error', () => {
    jobeServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultErr));

    component.functionDefinitions = 'test';
    const wrapped = wrapFunctions('csharp', component.functionDefinitions);

    component.onSubmit();
    expect(jobeServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('run error');
    expect(component.pendingSubmit).toBe(false);

    expect(jobeServerServiceSpy.setFunctionDefinitions).not.toHaveBeenCalledWith(component.functionDefinitions);
  });

  it('should clear code when changed', () => {
    component.functionDefinitions = 'test';
    component.modelChanged();

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(true);

    expect(jobeServerServiceSpy.clearFunctionDefinitions).toHaveBeenCalled();
  });

  it('should not allow empty code to be submitted', () => {

    component.modelChanged();

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(false);

    expect(jobeServerServiceSpy.clearFunctionDefinitions).toHaveBeenCalled();
  });

  it('should call checkRules on enter', () => {

    jobeServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOK));

    component.functionDefinitions = 'test';
    const wrapped = wrapFunctions('csharp', component.functionDefinitions);

    component.onSubmit();
    expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith("csharp", Applicability.functions, "test");
    expect(jobeServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.compiledOK).toBe(true);
    expect(component.currentStatus).toBe('Compiled OK');
    expect(component.pendingSubmit).toBe(false);

    expect(jobeServerServiceSpy.setFunctionDefinitions).toHaveBeenCalledWith(component.functionDefinitions);
  });

  it('should not submit on checkRules error', () => {

    jobeServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultOK));
    rulesServiceSpy.checkRules.and.returnValue("rules fail");

    component.functionDefinitions = 'test';

    component.onSubmit();
    expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith("csharp", Applicability.functions, "test");
    expect(jobeServerServiceSpy.submit_run).not.toHaveBeenCalled();

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('rules fail');
    expect(component.pendingSubmit).toBe(false);

    expect(jobeServerServiceSpy.setFunctionDefinitions).not.toHaveBeenCalledWith(component.functionDefinitions);
  });


  it('should disable paste by default', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });

  it('should enable paste from task', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteFunction: true } as ITask);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).not.toHaveBeenCalled();
  });

  it('should disable paste from task', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteFunction: false } as ITask);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });

  it('should show no code by default and disable Reset button', () => {

    expect(component.functionDefinitions).toEqual('');
    expect(component.skeleton).toEqual('');
  });

  it('should show skeleton code from task and enable Reset button', () => {

    taskSubject.next({ SkeletonCode: 'demo skeleton code' } as ITask);

    expect(component.functionDefinitions).toEqual('demo skeleton code');
    expect(component.skeleton).toEqual('demo skeleton code');
    expect(component.skeletonUnchanged).toBe(true);
  });

  it('should reset skeleton code on reset button', () => {

    taskSubject.next({ SkeletonCode: 'demo skeleton code' } as ITask);

    expect(component.functionDefinitions).toEqual('demo skeleton code');
    expect(component.skeleton).toEqual('demo skeleton code');
    expect(component.skeletonUnchanged).toBe(true);

    component.functionDefinitions = 'updated code';

    expect(component.functionDefinitions).toEqual('updated code');
    expect(component.skeleton).toEqual('demo skeleton code');
    expect(component.skeletonUnchanged).toBe(false);

    component.onReset();

    expect(component.functionDefinitions).toEqual('demo skeleton code');
    expect(component.skeleton).toEqual('demo skeleton code');
    expect(component.skeletonUnchanged).toBe(true);
  });

  it('gets the placeholder for the selected language', () => {
    expect(component.placeholder).toEqual('static [returnType] Name([parameter definitions]) => [expression];');
  });
});
