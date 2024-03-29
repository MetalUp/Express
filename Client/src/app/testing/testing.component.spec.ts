import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ITaskUserView } from '../models/task-user-view';
import { TaskService } from '../services/task.service';
import { of, Subject } from 'rxjs';
import { TestingComponent } from './testing.component';
import { RulesService } from '../services/rules.service';
import { EmptyRunResult, RunResult } from '../models/run-result';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { CompileServerService } from '../services/compile-server.service';

const testRunResultTestPass: RunResult = {
  run_id: 'a',
  outcome: 15,
  cmpinfo: '',
  stdout: 'All tests passed.',
  stderr: ''
};

const testRunResultTestFail: RunResult = {
  run_id: 'a',
  outcome: 12,
  cmpinfo: '',
  stdout: 'test failed',
  stderr: 'test failed error'
};

const testRunResultTestErr: RunResult = {
  run_id: 'a',
  outcome: 12,
  cmpinfo: '',
  stdout: '',
  stderr: 'run error'
};

const testRunResultTestCmp: RunResult = {
  run_id: 'a',
  outcome: 11,
  cmpinfo: 'compile error',
  stdout: '',
  stderr: ''
};

const testRunResultTestOutcome: RunResult = {
  run_id: 'a',
  outcome: 18,
  cmpinfo: '',
  stdout: '',
  stderr: ''
};


describe('TestingComponent', () => {
  let component: TestingComponent;
  let fixture: ComponentFixture<TestingComponent>;
  let compileServerServiceSpy: jasmine.SpyObj<CompileServerService>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let rulesServiceSpy: jasmine.SpyObj<RulesService>;
  const taskSubject = new Subject<ITaskUserView>();

  beforeEach(async () => {
    compileServerServiceSpy = jasmine.createSpyObj('CompileServerService', ['runTests', 'hasUserDefinedCode'], { "selectedLanguage": "csharp" });
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['loadTask'], { currentTask: taskSubject });
    rulesServiceSpy = jasmine.createSpyObj('RulesService', ['filter', 'filterAndReplace']);

    await TestBed.configureTestingModule({
      declarations: [TestingComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: CompileServerService,
          useValue: compileServerServiceSpy
        },
        {
          provide: TaskService,
          useValue: taskServiceSpy
        },
        {
          provide: RulesService,
          useValue: rulesServiceSpy
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TestingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    component.result = EmptyRunResult;
    component.tests = "tests";
    rulesServiceSpy.filter.and.callFake((_e, tf) => tf);
    rulesServiceSpy.filterAndReplace.and.callFake((_e) => _e);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should disable run tests until code compiled', () => {

    component.hasTests = true;
    compileServerServiceSpy.hasUserDefinedCode.and.returnValue(false);
    expect(component.canRunTests()).toEqual(false);
  });

  it('should enable run tests when code compiled', () => {
    component.hasTests = true;
    compileServerServiceSpy.hasUserDefinedCode.and.returnValue(true);
    expect(component.canRunTests()).toEqual(true);
  });

  it('should submit test code - test pass', () => {
    component.hasTests = true;
    compileServerServiceSpy.runTests.and.returnValue(of<RunResult>(testRunResultTestPass));
    compileServerServiceSpy.hasUserDefinedCode.and.returnValue(true);

    component.taskId = 56;

    component.onRunTests();

    expect(compileServerServiceSpy.runTests).toHaveBeenCalledWith(56);

    expect(component.currentResultMessage).toEqual('All tests passed.');
    expect(component.message()).toEqual('All tests passed.');
    expect(component.testedOk).toEqual(true);
    expect(taskServiceSpy.loadTask).toHaveBeenCalledOnceWith(56);
  });

  it('should submit test code - test fail', () => {
    component.hasTests = true;
    compileServerServiceSpy.runTests.and.returnValue(of<RunResult>(testRunResultTestFail));
    compileServerServiceSpy.hasUserDefinedCode.and.returnValue(true);

    component.taskId = 56;
  

    component.onRunTests();

    expect(compileServerServiceSpy.runTests).toHaveBeenCalledWith(56);

    expect(component.currentResultMessage).toEqual('test failed error');
    expect(component.message()).toEqual('test failed error');
    expect(component.testedOk).toEqual(false);
    expect(taskServiceSpy.loadTask).toHaveBeenCalledOnceWith(56);
  });

  it('should submit test code - test error', () => {
    component.hasTests = true;
    compileServerServiceSpy.runTests.and.returnValue(of<RunResult>(testRunResultTestErr));
    compileServerServiceSpy.hasUserDefinedCode.and.returnValue(true);
    

    component.taskId = 56;
   

    component.onRunTests();

    expect(compileServerServiceSpy.runTests).toHaveBeenCalledWith(56);

    expect(component.currentResultMessage).toEqual('run error');
    expect(component.message()).toEqual('run error');
    expect(component.testedOk).toEqual(false);
    expect(taskServiceSpy.loadTask).toHaveBeenCalledOnceWith(56);
  });

  it('should submit test code - test compile error', () => {
    component.hasTests = true;
    compileServerServiceSpy.runTests.and.returnValue(of<RunResult>(testRunResultTestCmp));
    compileServerServiceSpy.hasUserDefinedCode.and.returnValue(true);

    component.taskId = 56;
   

    component.onRunTests();

    expect(compileServerServiceSpy.runTests).toHaveBeenCalledWith(56);

    expect(component.currentResultMessage).toEqual("The Test system cannot find the function(s) it expects to see in your code. Check the function signature(s) carefully. If you can't see why a function signature is wrong, use a Hint. " + 'compile error');
    expect(component.message()).toEqual("The Test system cannot find the function(s) it expects to see in your code. Check the function signature(s) carefully. If you can't see why a function signature is wrong, use a Hint. " + 'compile error');
    expect(component.testedOk).toEqual(false);
    expect(taskServiceSpy.loadTask).toHaveBeenCalledOnceWith(56);
  });

  it('should submit test code - test outcome error', () => {
    component.hasTests = true;
    compileServerServiceSpy.runTests.and.returnValue(of<RunResult>(testRunResultTestOutcome));
    compileServerServiceSpy.hasUserDefinedCode.and.returnValue(true);

    component.taskId = 56;
   

    component.onRunTests();

    expect(compileServerServiceSpy.runTests).toHaveBeenCalledWith(56);

    expect(component.currentResultMessage).toEqual('Unknown or pending outcome');
    expect(component.message()).toEqual('Unknown or pending outcome');
    expect(component.testedOk).toEqual(false);
    expect(taskServiceSpy.loadTask).toHaveBeenCalledOnceWith(56);
  });

  it('should allow testing when jobe server has test functions', () => {
    
    component.hasTests = true;
    compileServerServiceSpy.hasUserDefinedCode.and.returnValue(true);
    component.testedOk = true;
    component.result = EmptyRunResult;

    expect(component.canRunTests()).toEqual(true);
    expect(component.testedOk).toBe(true);
    expect(component.currentResultMessage).toEqual('');
    expect(component.result.outcome).toBe(0);
  });

  it('should not allow testing when jobe server has no test functions', () => {
    
    component.hasTests = true;
    compileServerServiceSpy.hasUserDefinedCode.and.returnValue(false);

    component.testedOk = true;
    component.result = EmptyRunResult;

    expect(component.canRunTests()).toEqual(false);
    expect(component.testedOk).toBe(false);
    expect(component.currentResultMessage).toEqual('');
    expect(component.result.outcome).toBe(0);
  });

  it('should not allow testing when tests run', () => {
    
    component.hasTests = true;
    compileServerServiceSpy.hasUserDefinedCode.and.returnValue(false);
    component.testedOk = true;
    component.result = testRunResultTestOutcome;

    expect(component.canRunTests()).toEqual(false);
    expect(component.testedOk).toBe(false);
    expect(component.currentResultMessage).toEqual('');
    expect(component.result.outcome).toBe(0);
  });

  it('should not allow testing when no tests', () => {
    
    component.hasTests = false;
    compileServerServiceSpy.hasUserDefinedCode.and.returnValue(true);

    component.testedOk = true;
    component.result = EmptyRunResult;

    expect(component.canRunTests()).toEqual(false);
    expect(component.testedOk).toBe(true);
    expect(component.currentResultMessage).toEqual('');
    expect(component.result.outcome).toBe(0);
  });

});
