import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RunResult } from '../models/run-result';
import { of, Subject } from 'rxjs';
import { CodeDefinitionComponent } from './code-definition.component';
import { RulesService } from '../services/rules.service';
import { Applicability } from '../models/rules';
import { TaskService } from '../services/task.service';
import { ITaskUserView } from '../models/task';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { CompileServerService } from '../services/compile-server.service';

describe('CodeDefinitionComponent', () => {
  let component: CodeDefinitionComponent;
  let fixture: ComponentFixture<CodeDefinitionComponent>;
  let compileServerServiceSpy: jasmine.SpyObj<CompileServerService>;
  let rulesServiceSpy: jasmine.SpyObj<RulesService>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITaskUserView>();

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
    compileServerServiceSpy = jasmine.createSpyObj('CompileServerService', ['submitCode', 'clearUserDefinedCode', 'setUserDefinedCode'], { "selectedLanguage": "csharp" });

    rulesServiceSpy = jasmine.createSpyObj('RulesService', ['filter', 'checkRules']);
    rulesServiceSpy.checkRules.and.returnValue('');
    rulesServiceSpy.filter.and.callFake((_e, tf) => tf);

    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], { currentTask: taskSubject });

    await TestBed.configureTestingModule({
      declarations: [CodeDefinitionComponent],
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

    fixture = TestBed.createComponent(CodeDefinitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });


  it('should submit code for compile OK', () => {
    compileServerServiceSpy.submitCode.and.returnValue(of<RunResult>(testRunResultOK));

    component.taskId = 67;
    component.codeDefinitions = 'test';
    

    component.onSubmit();
    expect(compileServerServiceSpy.submitCode).toHaveBeenCalledWith(67, 'test');

    expect(component.compiledOK).toBe(true);
    expect(component.currentStatus).toBe('Compiled OK');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.setUserDefinedCode).toHaveBeenCalledWith(component.codeDefinitions);

  });

  it('should submit code for compile Fail', () => {
    compileServerServiceSpy.submitCode.and.returnValue(of<RunResult>(testRunResultCmp));

    component.taskId = 67;
    component.codeDefinitions = 'test';
   

    component.onSubmit();
    expect(compileServerServiceSpy.submitCode).toHaveBeenCalledWith(67, 'test');

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('compiler error');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.setUserDefinedCode).not.toHaveBeenCalledWith(component.codeDefinitions);

  });

  it('should submit code for compile Error', () => {
    compileServerServiceSpy.submitCode.and.returnValue(of<RunResult>(testRunResultErr));

    component.taskId = 67;
    component.codeDefinitions = 'test';

    component.onSubmit();
    expect(compileServerServiceSpy.submitCode).toHaveBeenCalledWith(67, 'test');

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('run error');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.setUserDefinedCode).not.toHaveBeenCalledWith(component.codeDefinitions);
  });

  it('should clear code when changed', () => {

    component.taskId = 67;
    component.codeDefinitions = 'test';
    component.modelChanged();

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(true);

    expect(compileServerServiceSpy.clearUserDefinedCode).toHaveBeenCalled();
  });

  // it('should call model changed when changed', () => {

  //   component.taskId = 67;
  //   component.nextTaskClears = true;
  //   component.codeDefinitions = 'something';
    
  //   taskSubject.next({ Id: 1, PasteCode: true } as ITaskUserView);

  //   expect(component.compiledOK).toBe(false);
  //   expect(component.currentStatus).toBe('');
  //   expect(component.pendingSubmit).toBe(false);
  //   expect(component.codeDefinitions).toBe('');


  //   expect(compileServerServiceSpy.clearUserDefinedCode).toHaveBeenCalled();
  // });

  it('should not call model changed when changed in nextClassClears flag unset', () => {

    component.taskId = 67;
    component.codeDefinitions = 'original';
    component.nextTaskClears = false;
    component.compiledOK = true;
    
    taskSubject.next({ Id: 1, PasteCode: true } as ITaskUserView);

    expect(component.codeDefinitions).toBe('original');
    expect(component.compiledOK).toBe(true);
    expect(component.currentStatus).toBe('Compiled OK');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.clearUserDefinedCode).not.toHaveBeenCalled();
  });

  // it('should default nextClassClears flag', () => {
  //   expect(component.nextTaskClears).toBe(true);
  //   taskSubject.next({ } as ITask);
  //   expect(component.nextTaskClears).toBe(false);
  // });

  it('should unset nextClassClears flag from task', () => {
    expect(component.nextTaskClears).toBe(true);
    taskSubject.next({ NextTaskClearsFunctions: true} as ITaskUserView);
    expect(component.nextTaskClears).toBe(true);
  });

  it('should set nextClassClears flag from task', () => {
    component.nextTaskClears = false;
    expect(component.nextTaskClears).toBe(false);
    taskSubject.next({ NextTaskClearsFunctions: false} as ITaskUserView);
    expect(component.nextTaskClears).toBe(false);
  });


  it('should not allow empty code to be submitted', () => {

    component.modelChanged();

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.clearUserDefinedCode).toHaveBeenCalled();
  });

  it('should call checkRules on enter', () => {

    compileServerServiceSpy.submitCode.and.returnValue(of<RunResult>(testRunResultOK));

    component.taskId = 67;
    component.codeDefinitions = 'test';
    

    component.onSubmit();
    expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith(Applicability.functions, "test");
    expect(compileServerServiceSpy.submitCode).toHaveBeenCalledWith(67, 'test');

    expect(component.compiledOK).toBe(true);
    expect(component.currentStatus).toBe('Compiled OK');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.setUserDefinedCode).toHaveBeenCalledWith(component.codeDefinitions);
  });

  it('should not submit on checkRules error', () => {

    compileServerServiceSpy.submitCode.and.returnValue(of<RunResult>(testRunResultOK));
    rulesServiceSpy.checkRules.and.returnValue("rules fail");

    component.taskId = 67;
    component.codeDefinitions = 'test';

    component.onSubmit();
    expect(rulesServiceSpy.checkRules).toHaveBeenCalledWith(Applicability.functions, "test");
    expect(compileServerServiceSpy.submitCode).not.toHaveBeenCalled();

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('rules fail');
    expect(component.pendingSubmit).toBe(false);

    expect(compileServerServiceSpy.setUserDefinedCode).not.toHaveBeenCalledWith(component.codeDefinitions);
  });


  it('should disable paste by default', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });

  it('should enable paste from task', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteCode: true } as ITaskUserView);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).not.toHaveBeenCalled();
  });

  it('should disable paste from task', () => {

    let eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteCode: false } as ITaskUserView);

    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });

  it('should set taskid from task', () => {

    taskSubject.next({ Id: 65 } as ITaskUserView);

    expect(component.taskId).toEqual(65);
  });

  it('should show no code by default and disable Reset button', () => {

    expect(component.codeDefinitions).toEqual('');
  });

  it('gets the placeholder for the selected language', () => {
    expect(component.placeholder).toEqual('static <returnType> Name(<parameter definitions>) => <expression>;');
  });
});
