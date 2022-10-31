import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RunResult } from '../models/run-result';
import { of, Subject } from 'rxjs';
import { FunctionDefinitionComponent } from './function-definition.component';
import { RulesService } from '../services/rules.service';
import { Applicability } from '../models/rules';
import { TaskService } from '../services/task.service';
import { ITask } from '../models/task';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { CompileServerService } from '../services/compile-server.service';

describe('FunctionDefinitionComponent', () => {
  let component: FunctionDefinitionComponent;
  let fixture: ComponentFixture<FunctionDefinitionComponent>;
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
    compileServerServiceSpy = jasmine.createSpyObj('CompileServerService', ['submitCode', 'clearFunctionDefinitions', 'setFunctionDefinitions'], { "selectedLanguage": "csharp" });

    rulesServiceSpy = jasmine.createSpyObj('RulesService', ['filter', 'checkRules']);
    rulesServiceSpy.checkRules.and.returnValue('');
    rulesServiceSpy.filter.and.callFake((_l, _e, tf) => tf);

    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], { currentTask: taskSubject });

    await TestBed.configureTestingModule({
      declarations: [FunctionDefinitionComponent],
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
    compileServerServiceSpy.submitCode.and.returnValue(of<RunResult>(testRunResultOK));

    component.taskId = 67;
    component.functionDefinitions = 'test';
    

    component.onSubmit();
    expect(compileServerServiceSpy.submitCode).toHaveBeenCalledWith(67, 'test');

    expect(component.compiledOK).toBe(true);
    expect(component.currentStatus).toBe('Compiled OK');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.setFunctionDefinitions).toHaveBeenCalledWith(component.functionDefinitions);

  });

  it('should submit code for compile Fail', () => {
    compileServerServiceSpy.submitCode.and.returnValue(of<RunResult>(testRunResultCmp));

    component.taskId = 67;
    component.functionDefinitions = 'test';
   

    component.onSubmit();
    expect(compileServerServiceSpy.submitCode).toHaveBeenCalledWith(67, 'test');

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('compiler error');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.setFunctionDefinitions).not.toHaveBeenCalledWith(component.functionDefinitions);

  });

  it('should submit code for compile Error', () => {
    compileServerServiceSpy.submitCode.and.returnValue(of<RunResult>(testRunResultErr));

    component.taskId = 67;
    component.functionDefinitions = 'test';

    component.onSubmit();
    expect(compileServerServiceSpy.submitCode).toHaveBeenCalledWith(67, 'test');

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('run error');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.setFunctionDefinitions).not.toHaveBeenCalledWith(component.functionDefinitions);
  });

  it('should clear code when changed', () => {

    component.taskId = 67;
    component.functionDefinitions = 'test';
    component.modelChanged();

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(true);

    expect(compileServerServiceSpy.clearFunctionDefinitions).toHaveBeenCalled();
  });

  it('should call model changed when changed', () => {

    component.taskId = 67;
    component.functionDefinitions = '';
    
    taskSubject.next({ PasteFunctions: true, SkeletonCode: 'test' } as ITask);

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(true);

    expect(compileServerServiceSpy.clearFunctionDefinitions).toHaveBeenCalled();
  });

  it('should not call model changed when changed in nextClassClears flag unset', () => {

    component.taskId = 67;
    component.functionDefinitions = 'original';
    component.nextTaskClears = false;
    component.compiledOK = true;
    
    taskSubject.next({ PasteFunctions: true, SkeletonCode: 'test' } as ITask);

    expect(component.functionDefinitions).toBe('original');
    expect(component.compiledOK).toBe(true);
    expect(component.currentStatus).toBe('Compiled OK');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.clearFunctionDefinitions).not.toHaveBeenCalled();
  });

  it('should default nextClassClears flag', () => {
    expect(component.nextTaskClears).toBe(true);
    taskSubject.next({ } as ITask);
    expect(component.nextTaskClears).toBe(false);
  });

  it('should unset nextClassClears flag from task', () => {
    expect(component.nextTaskClears).toBe(true);
    taskSubject.next({ NextTaskClearsFunctions: true} as ITask);
    expect(component.nextTaskClears).toBe(true);
  });

  it('should set nextClassClears flag from task', () => {
    component.nextTaskClears = false;
    expect(component.nextTaskClears).toBe(false);
    taskSubject.next({ NextTaskClearsFunctions: false} as ITask);
    expect(component.nextTaskClears).toBe(false);
  });


  it('should not allow empty code to be submitted', () => {

    component.modelChanged();

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.clearFunctionDefinitions).toHaveBeenCalled();
  });

  it('should call checkRules on enter', () => {

    compileServerServiceSpy.submitCode.and.returnValue(of<RunResult>(testRunResultOK));

    component.taskId = 67;
    component.functionDefinitions = 'test';
    

    component.onSubmit();
    expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith("csharp", Applicability.functions, "test");
    expect(compileServerServiceSpy.submitCode).toHaveBeenCalledWith(67, 'test');

    expect(component.compiledOK).toBe(true);
    expect(component.currentStatus).toBe('Compiled OK');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.setFunctionDefinitions).toHaveBeenCalledWith(component.functionDefinitions);
  });

  it('should not submit on checkRules error', () => {

    compileServerServiceSpy.submitCode.and.returnValue(of<RunResult>(testRunResultOK));
    rulesServiceSpy.checkRules.and.returnValue("rules fail");

    component.taskId = 67;
    component.functionDefinitions = 'test';

    component.onSubmit();
    expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith("csharp", Applicability.functions, "test");
    expect(compileServerServiceSpy.submitCode).not.toHaveBeenCalled();

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('rules fail');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.setFunctionDefinitions).not.toHaveBeenCalledWith(component.functionDefinitions);
  });


  it('should disable paste by default', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });

  it('should enable paste from task', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteFunctions: true } as ITask);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).not.toHaveBeenCalled();
  });

  it('should disable paste from task', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteFunctions: false } as ITask);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });

  it('should set taskid from task', () => {

    taskSubject.next({ Id: 65 } as ITask);

    expect(component.taskId).toEqual(65);
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
    expect(component.placeholder).toEqual('static <returnType> Name(<parameter definitions>) => <expression>;');
  });
});
