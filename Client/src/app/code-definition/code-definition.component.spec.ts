import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { RunResult } from '../models/run-result';
import { of, Subject } from 'rxjs';
import { CodeDefinitionComponent } from './code-definition.component';
import { RulesService } from '../services/rules.service';
import { Applicability } from '../models/rules';
import { TaskService } from '../services/task.service';
import { ITaskUserView } from '../models/task-user-view';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { CompileServerService } from '../services/compile-server.service';
import { EmptyCodeUserView, ICodeUserView } from '../models/code-user-view';

describe('CodeDefinitionComponent', () => {
  let component: CodeDefinitionComponent;
  let fixture: ComponentFixture<CodeDefinitionComponent>;
  let compileServerServiceSpy: jasmine.SpyObj<CompileServerService>;
  let rulesServiceSpy: jasmine.SpyObj<RulesService>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  const taskSubject = new Subject<ITaskUserView>();

  const testRunResultOK: RunResult = {
    run_id: 'a',
    outcome: 15,
    cmpinfo: '',
    stdout: 'expression result',
    stderr: ''
  };

  const testRunResultCmp: RunResult = {
    run_id: 'a',
    outcome: 11,
    cmpinfo: 'compiler error',
    stdout: '',
    stderr: ''
  };

  const testRunResultErr: RunResult = {
    run_id: 'a',
    outcome: 12,
    cmpinfo: '',
    stdout: '',
    stderr: 'run error'
  };


  beforeEach(async () => {
    compileServerServiceSpy = jasmine.createSpyObj('CompileServerService', ['submitCode', 'clearUserDefinedCode', 'setUserDefinedCode'], { "selectedLanguage": "csharp" });

    rulesServiceSpy = jasmine.createSpyObj('RulesService', ['filter', 'checkRules']);
    rulesServiceSpy.checkRules.and.returnValue('');
    rulesServiceSpy.filter.and.callFake((_e, tf) => tf);

    taskServiceSpy = jasmine.createSpyObj('TaskService', ['loadTask', 'loadCode', 'loadHint'], { currentTask: taskSubject });

    const testCode = EmptyCodeUserView;

    taskServiceSpy.loadCode.and.returnValue(Promise.resolve(testCode));

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
    component.unsubmittedCode = "test1";

    component.onSubmit();
    expect(compileServerServiceSpy.submitCode).toHaveBeenCalledWith(67, 'test');
    expect(component.compiledOK).toBe(true);
    expect(component.currentStatus).toBe('Compiled OK');
    expect(component.pendingSubmit).toBe(false);
    expect(compileServerServiceSpy.setUserDefinedCode).toHaveBeenCalledWith(component.codeDefinitions);
    expect(component.unsubmittedCode).toBe("");
    expect(taskServiceSpy.loadCode).toHaveBeenCalledWith(67, 1);

  });

  it('should submit code for compile Fail', () => {
    compileServerServiceSpy.submitCode.and.returnValue(of<RunResult>(testRunResultCmp));

    component.taskId = 67;
    component.codeDefinitions = 'test';
    component.unsubmittedCode = "test1";

    component.onSubmit();
    expect(compileServerServiceSpy.submitCode).toHaveBeenCalledWith(67, 'test');
    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('compiler error');
    expect(component.pendingSubmit).toBe(false);
    expect(compileServerServiceSpy.setUserDefinedCode).not.toHaveBeenCalledWith(component.codeDefinitions);
    expect(component.unsubmittedCode).toBe("test1");
    expect(taskServiceSpy.loadCode).not.toHaveBeenCalled();
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

  it('should call loadCode when task changed', () => {

    component.taskId = 67;
    
    taskSubject.next({ Id: 1, PasteCode: true, Language: "lang" } as ITaskUserView);

    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(false);

    expect(taskServiceSpy.loadCode).toHaveBeenCalledWith(1, 0);
  });

  it('should not call load code when task refreshed', () => {

    component.taskId = 67;
    component.codeDefinitions = 'something';

    taskSubject.next({ Id: 67, PasteCode: true, Code: '',Language: "lang" } as ITaskUserView);

    expect(component.codeDefinitions).toBe('something');

    expect(taskServiceSpy.loadCode).not.toHaveBeenCalled();
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
    const eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });

  it('should enable paste from task', () => {
    const eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteCode: true, Language: "lang" } as ITaskUserView);
    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).not.toHaveBeenCalled();
  });

  it('should disable paste from task', () => {
    const eventSpy = jasmine.createSpyObj('ClipboardEvent', ['preventDefault']);
    taskSubject.next({ PasteCode: false, Language: "lang" } as ITaskUserView);
    component.onPaste(eventSpy);
    expect(eventSpy.preventDefault).toHaveBeenCalled();
  });

  it('should set taskid from task', () => {
    taskSubject.next({ Id: 65, Language: "lang" } as ITaskUserView);
    expect(component.taskId).toEqual(65);
  });

  it('should show no code by default', () => {
    expect(component.codeDefinitions).toEqual('');
  });

  it('should get unsubmitted code ', fakeAsync(() => {

    component.currentCodeVersion = { Version: 1 } as unknown as ICodeUserView;
    component.codeDefinitions = 'something';
    component.unsubmittedCode = 'unsubmitted';
  
    expect(component.canNewerCode()).toBeTrue();
    component.newerCode();
   
    expect(component.codeDefinitions).toBe('unsubmitted');
    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(true);
    expect(component.currentCodeVersion.Version).toBe(0);
  }));

  it('should get newer code ', fakeAsync(() => {

    const testCodeVersion : ICodeUserView = { TaskId: 0, Version: 1, Code: "new code", HasPreviousVersion: true};

    taskServiceSpy.loadCode.and.returnValue(Promise.resolve(testCodeVersion));

    component.taskId = 1;
    component.currentCodeVersion = { Version: 2 } as unknown as ICodeUserView;
    component.codeDefinitions = 'something';
    component.unsubmittedCode = 'unsubmitted';
  
    expect(component.canNewerCode()).toBeTrue();
    component.newerCode();
    tick();
    expect(taskServiceSpy.loadCode).toHaveBeenCalledWith(1, 1);
   
    expect(component.codeDefinitions).toBe('new code');
    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(true);
    expect(component.currentCodeVersion.Version).toBe(1);
  }));

  it('should get older code ', fakeAsync(() => {

    const testCodeVersion : ICodeUserView = { TaskId: 0, Version: 3, Code: "old code", HasPreviousVersion: true};

    taskServiceSpy.loadCode.and.returnValue(Promise.resolve(testCodeVersion));

    component.taskId = 1;
    component.currentCodeVersion = { Version: 2, HasPreviousVersion: true } as unknown as ICodeUserView;
    component.codeDefinitions = 'something';
    component.unsubmittedCode = 'unsubmitted';
  
    expect(component.canOlderCode()).toBeTrue();
    component.olderCode();
    tick();
    expect(taskServiceSpy.loadCode).toHaveBeenCalledWith(1, 3);
   
    expect(component.codeDefinitions).toBe('old code');
    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(true);
    expect(component.currentCodeVersion.Version).toBe(3);
  }));
});
