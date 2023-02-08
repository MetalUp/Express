import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { ActionResultRepresentation, DomainObjectRepresentation, DomainServicesRepresentation, IActionInvokeRepresentation, Value } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { Subject } from 'rxjs';
import { RunResult, errorRunResult } from '../models/run-result';
import { ITaskUserView } from '../models/task-user-view';

import { CompileServerService } from './compile-server.service';
import { TaskService } from './task.service';

describe('CompileServerService', () => {
  let service: CompileServerService;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITaskUserView>();

  let testRunResult: RunResult = {
    run_id: 'a',
    outcome: 66,
    cmpinfo: 'b',
    stdout: 'c',
    stderr: 'd',
    lineno: 0,
    colno: 0
  }

  class mockValue {
    constructor(private val: any) { }
    value = () => ({
      scalar: () => this.val
    })
  }

  let mockAR = {
    result: () => ({
      object: () => ({
        propertyMember: (n: string) => {
          switch (n) {
            case "Cmpinfo": return new mockValue("b");
            case "Outcome": return new mockValue(66);
            case "Stderr": return new mockValue("d");
            case "Stdout": return new mockValue("c");
            case "RunID": return new mockValue("a");
            case 'LineNo': return new mockValue(0);
            case 'ColNo': return new mockValue(0);
          }
          return new mockValue("fail");
        }
      }),
      list: () => ({
        value: () => []
      })
    })
  } as unknown as ActionResultRepresentation;

  let mockAction = {
  } as IActionInvokeRepresentation;

  let mockService = {
    actionMember: (n: string) => mockAction
  } as unknown as DomainObjectRepresentation;

  let mockServices = {
    getService: () => mockService
  } as unknown as DomainServicesRepresentation;

  beforeEach(fakeAsync(() => {
    contextServiceSpy = jasmine.createSpyObj('ContextService', ['getServices']);
    repLoaderSpy = jasmine.createSpyObj('RepLoaderService', ['invoke', 'populate']);
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load', 'getFile'], { currentTask: taskSubject });
    contextServiceSpy.getServices.and.returnValue(Promise.resolve(mockServices));
    repLoaderSpy.invoke.and.returnValue(Promise.resolve(mockAR));
    repLoaderSpy.populate.and.returnValue(Promise.resolve(mockService));


    TestBed.configureTestingModule({});
    service = new CompileServerService(taskServiceSpy, repLoaderSpy, contextServiceSpy);
    tick();
    service.selectedLanguage = 'stub language';
    service.clearUserDefinedCode();
  }));

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call evaluateExpression', fakeAsync(() => {
    tick();
    const promise = Promise.resolve(mockAR);
    repLoaderSpy.invoke.and.returnValue(promise);
    service.evaluateExpression(46, "stub code").subscribe(o => expect(o).toEqual(testRunResult));
    var params = { "taskId": new Value(46), "expression": new Value("stub code"), "code": new Value("") } as Dictionary<Value>;

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.evaluateExpressionAction, params, service.urlParams);
  }));

  it('should call evaluateExpression and include user functions', fakeAsync(() => {
    tick();
    const promise = Promise.resolve(mockAR);
    repLoaderSpy.invoke.and.returnValue(promise);

    service.setUserDefinedCode("extra code");
    service.evaluateExpression(46, "stub code").subscribe(o => expect(o).toEqual(testRunResult));
    var params = { "taskId": new Value(46), "expression": new Value("stub code"), "code": new Value("extra code") } as Dictionary<Value>;

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.evaluateExpressionAction, params, service.urlParams);
  }));

  it('should call submitCode', fakeAsync(() => {
    tick();
    const promise = Promise.resolve(mockAR);
    repLoaderSpy.invoke.and.returnValue(promise);
    service.submitCode(46, "stub code").subscribe(o => expect(o).toEqual(testRunResult));

    var params = { "taskId": new Value(46), "code": new Value("stub code") } as Dictionary<Value>;

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.submitCodeAction, params, service.urlParams);
  }));

  it('should call submitCode and not include user code', fakeAsync(() => {
    tick();
    const promise = Promise.resolve(mockAR);
    service.setUserDefinedCode("extra code");
    repLoaderSpy.invoke.and.returnValue(promise);
    service.submitCode(46, "stub code").subscribe(o => expect(o).toEqual(testRunResult));

    var params = { "taskId": new Value(46), "code": new Value("stub code") } as Dictionary<Value>;

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.submitCodeAction, params, service.urlParams);
  }));

  it('should call runTests', fakeAsync(() => {
    tick();
    const promise = Promise.resolve(mockAR);
    repLoaderSpy.invoke.and.returnValue(promise);
    service.runTests(46).subscribe(o => expect(o).toEqual(testRunResult));

    var params = { "taskId": new Value(46), "code": new Value("") } as Dictionary<Value>;

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.runTestsAction, params, service.urlParams);
  }));

  it('should call runTests and include user code', fakeAsync(() => {
    tick();
    const promise = Promise.resolve(mockAR);

    service.setUserDefinedCode("extra code");
    repLoaderSpy.invoke.and.returnValue(promise);
    service.runTests(46).subscribe(o => expect(o).toEqual(testRunResult));

    var params = { "taskId": new Value(46), "code": new Value("extra code") } as Dictionary<Value>;

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.runTestsAction, params, service.urlParams);
  }));

  it('should call evaluateExpression and return an empty result on error', fakeAsync(() => {
    repLoaderSpy.invoke.and.returnValue(Promise.reject(() => { status: 404 }));
    const unknownError = errorRunResult(null);

    service.evaluateExpression(46, "stub code").subscribe(o => expect(o).toEqual(unknownError));

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.evaluateExpressionAction, service.params(46, "stub code"), service.urlParams);
  }));

  it('should call submitCode and return an empty result on error', fakeAsync(() => {
    repLoaderSpy.invoke.and.returnValue(Promise.reject(() => { status: 404 }));
    const unknownError = errorRunResult(null);

    service.submitCode(46, "stub code").subscribe(o => expect(o).toEqual(unknownError));

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.evaluateExpressionAction, service.params(46, undefined, "stub code"), service.urlParams);
  }));

  it('should call runTests and return an empty result on error', fakeAsync(() => {
    repLoaderSpy.invoke.and.returnValue(Promise.reject(() => { status: 404 }));
    const unknownError = errorRunResult(null);

    service.runTests(46).subscribe(o => expect(o).toEqual(unknownError));

    expect(repLoaderSpy.invoke).toHaveBeenCalledWith(service.evaluateExpressionAction, service.params(46, ""), service.urlParams);
  }));
});
