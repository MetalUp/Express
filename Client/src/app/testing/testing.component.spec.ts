import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { EmptyTask, ITask } from '../models/task';
import { TaskService } from '../services/task.service';
import { of, Subject } from 'rxjs';
import { TestingComponent } from './testing.component';
import { RulesService } from '../services/rules.service';
import { EmptyRunResult, RunResult } from '../models/run-result';
import { wrapTests } from '../language-helpers/language-helpers';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { CompileServerService } from '../services/compile-server.service';

let testRunResultTestPass: RunResult = {
  run_id: 'a',
  outcome: 15,
  cmpinfo: '',
  stdout: 'All tests passed.',
  stderr: ''
}

let testRunResultTestFail: RunResult = {
  run_id: 'a',
  outcome: 12,
  cmpinfo: '',
  stdout: 'test failed',
  stderr: 'test failed error'
}

let testRunResultTestErr: RunResult = {
  run_id: 'a',
  outcome: 12,
  cmpinfo: '',
  stdout: '',
  stderr: 'run error'
}

let testRunResultTestCmp: RunResult = {
  run_id: 'a',
  outcome: 11,
  cmpinfo: 'compile error',
  stdout: '',
  stderr: ''
}

let testRunResultTestOutcome: RunResult = {
  run_id: 'a',
  outcome: 18,
  cmpinfo: '',
  stdout: '',
  stderr: ''
}



describe('TestingComponent', () => {
  let component: TestingComponent;
  let fixture: ComponentFixture<TestingComponent>;
  let compileServerServiceSpy: jasmine.SpyObj<CompileServerService>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let rulesServiceSpy: jasmine.SpyObj<RulesService>;
  let taskSubject = new Subject<ITask>();

  beforeEach(async () => {
    compileServerServiceSpy = jasmine.createSpyObj('CompileServerService', ['submit_run', 'hasFunctionDefinitions'], { "selectedLanguage": "csharp" });
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load', 'getFile'], { currentTask: taskSubject });
    rulesServiceSpy = jasmine.createSpyObj('RulesService', ['filter']);

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
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should get the test code file', fakeAsync(() => {

    taskServiceSpy.getFile.and.returnValue(Promise.resolve('test code'));

    const testTask = structuredClone(EmptyTask) as ITask;
    testTask.Tests = ["testUrl", "testMt"];

    taskSubject.next(testTask);

    expect(taskServiceSpy.getFile).toHaveBeenCalledWith(["testUrl", "testMt"]);
    tick();
    expect(component.tests).toEqual('test code');
  }));

  it('should disable run tests until code compiled', () => {

    compileServerServiceSpy.hasFunctionDefinitions.and.returnValue(false);
    expect(component.canRunTests()).toEqual(false);
  });

  it('should enable run tests when code compiled', () => {

    compileServerServiceSpy.hasFunctionDefinitions.and.returnValue(true);
    expect(component.canRunTests()).toEqual(true);
  });

  it('should submit test code - test pass', () => {
    compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultTestPass));
    compileServerServiceSpy.hasFunctionDefinitions.and.returnValue(true);

    component.tests = 'test code';
    const wrapped = wrapTests('csharp', 'test code');

    component.onRunTests();

    expect(compileServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.currentResultMessage).toEqual('All tests passed.');
    expect(component.message()).toEqual('All tests passed.');
    expect(component.testedOk).toEqual(true);
  });

  it('should submit test code - test fail', () => {
    compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultTestFail));
    compileServerServiceSpy.hasFunctionDefinitions.and.returnValue(true);

    component.tests = 'test code';
    const wrapped = wrapTests('csharp', 'test code');

    component.onRunTests();

    expect(compileServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.currentResultMessage).toEqual('test failed');
    expect(component.message()).toEqual('test failed');
    expect(component.testedOk).toEqual(false);
  });

  it('should submit test code - test error', () => {
    compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultTestErr));
    compileServerServiceSpy.hasFunctionDefinitions.and.returnValue(true);
    rulesServiceSpy.filter.and.callFake((_l, _e, tf) => tf);

    component.tests = 'test code';
    const wrapped = wrapTests('csharp', 'test code');

    component.onRunTests();

    expect(compileServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.currentResultMessage).toEqual('run error');
    expect(component.message()).toEqual('run error');
    expect(component.testedOk).toEqual(false);
  });

  it('should submit test code - test compile error', () => {
    compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultTestCmp));
    compileServerServiceSpy.hasFunctionDefinitions.and.returnValue(true);


    component.tests = 'test code';
    const wrapped = wrapTests('csharp', 'test code');

    component.onRunTests();

    expect(compileServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.currentResultMessage).toEqual("The Test system cannot find the function(s) it expects to see in your code. Check the function signature(s) carefully. If you can't see why a function signature is wrong, use a Hint. " + 'compile error');
    expect(component.message()).toEqual("The Test system cannot find the function(s) it expects to see in your code. Check the function signature(s) carefully. If you can't see why a function signature is wrong, use a Hint. " + 'compile error');
    expect(component.testedOk).toEqual(false);
  });

  it('should submit test code - test outcome error', () => {
    compileServerServiceSpy.submit_run.and.returnValue(of<RunResult>(testRunResultTestOutcome));
    compileServerServiceSpy.hasFunctionDefinitions.and.returnValue(true);

    component.tests = 'test code';
    const wrapped = wrapTests('csharp', 'test code');

    component.onRunTests();

    expect(compileServerServiceSpy.submit_run).toHaveBeenCalledWith(wrapped);

    expect(component.currentResultMessage).toEqual('Unknown or pending outcome');
    expect(component.message()).toEqual('Unknown or pending outcome');
    expect(component.testedOk).toEqual(false);
  });

  it('should allow testing when jobe server has test functions', () => {
    
    compileServerServiceSpy.hasFunctionDefinitions.and.returnValue(true);
    component.testedOk = true;
    component.result = EmptyRunResult;

    expect(component.canRunTests()).toEqual(true);
    expect(component.testedOk).toBe(true);
    expect(component.currentResultMessage).toEqual('');
    expect(component.result.outcome).toBe(0);
  });

  it('should not allow testing when jobe server has no test functions', () => {
    
    compileServerServiceSpy.hasFunctionDefinitions.and.returnValue(false);

    component.testedOk = true;
    component.result = EmptyRunResult;

    expect(component.canRunTests()).toEqual(false);
    expect(component.testedOk).toBe(false);
    expect(component.currentResultMessage).toEqual('');
    expect(component.result.outcome).toBe(0);
  });

  it('should not allow testing when tests run', () => {
    
    compileServerServiceSpy.hasFunctionDefinitions.and.returnValue(false);
    component.testedOk = true;
    component.result = testRunResultTestOutcome;

    expect(component.canRunTests()).toEqual(false);
    expect(component.testedOk).toBe(false);
    expect(component.currentResultMessage).toEqual('');
    expect(component.result.outcome).toBe(0);
  });

});
