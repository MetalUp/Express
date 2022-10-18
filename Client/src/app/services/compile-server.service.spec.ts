import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { ActionResultRepresentation, DomainObjectRepresentation, DomainServicesRepresentation, IActionInvokeRepresentation, PropertyMember, Result, Value } from '@nakedobjects/restful-objects';
import { ContextService, RepLoaderService } from '@nakedobjects/services';
import { Dictionary } from 'lodash';
import { Subject } from 'rxjs';
import { UserDefinedFunctionPlaceholder, ReadyMadeFunctionsPlaceholder } from '../language-helpers/language-helpers';
import { RunResult, errorRunResult } from '../models/run-result';
import { ITask } from '../models/task';

import { CompileServerService } from './compile-server.service';
import { TaskService } from './task.service';

describe('CompileServerService', () => {
  let service: CompileServerService;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();

  let testRunSpec: Dictionary<Value>;
  let testRunResult: RunResult = {
    run_id: 'a',
    outcome: 66,
    cmpinfo: 'b',
    stdout: 'c',
    stderr: 'd'
  }

  class mockValue {
    constructor(private val: any) {}
    value = () => ({
      scalar: () => this.val
    })
  }


  let mockAR = {
    result : () => ({
      object: () => ({
        propertyMember: (n : string) => {
          switch(n){
            case "Cmpinfo": return new mockValue("b");
            case "Outcome": return new mockValue(66);
            case "Stderr": return new mockValue("d");
            case "Stdout": return new mockValue("c");
            case "RunID": return new mockValue("a");
          }
          return new mockValue("fail");
        } 
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
    taskServiceSpy.getFile.and.returnValue(Promise.resolve('additional task code'));
    contextServiceSpy.getServices.and.returnValue(Promise.resolve(mockServices));
    repLoaderSpy.invoke.and.returnValue(Promise.resolve(mockAR));
    repLoaderSpy.populate.and.returnValue(Promise.resolve(mockService));


    TestBed.configureTestingModule({});
    service = new CompileServerService(taskServiceSpy, repLoaderSpy, contextServiceSpy);
    tick();
    service.selectedLanguage = 'stub language';
    testRunSpec = service.run_spec('stub code');
  }));

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call post on /runs', fakeAsync(() => {
    tick();
    const promise = Promise.resolve(mockAR);
    repLoaderSpy.invoke.and.returnValue(promise);
    service.selectedLanguage = 'stub language';
    service.submit_run("stub code").subscribe(o => expect(o).toEqual(testRunResult));

    expect(repLoaderSpy.invoke).toHaveBeenCalledOnceWith(service.action, service.params("stub code"), service.urlParams);
  }));

  it('should call post on /runs including any function definitions', fakeAsync(() => {
    const promise = Promise.resolve(mockAR);
    repLoaderSpy.invoke.and.returnValue(promise);
    service.selectedLanguage = 'stub language';
    service.setFunctionDefinitions('test definitions ');
    service.submit_run(`${UserDefinedFunctionPlaceholder}stub code`).subscribe(o => expect(o).toEqual(testRunResult));

    expect(repLoaderSpy.invoke).toHaveBeenCalledOnceWith(service.action, service.params("test definitions stub code"), service.urlParams);
  }));

  it('should call post on /runs excluding empty function definitions', fakeAsync(() => {
    const promise = Promise.resolve(mockAR);
    repLoaderSpy.invoke.and.returnValue(promise);
    service.selectedLanguage = 'stub language';
    service.clearFunctionDefinitions();
    service.submit_run(`${UserDefinedFunctionPlaceholder}stub code`).subscribe(o => expect(o).toEqual(testRunResult));

    expect(repLoaderSpy.invoke).toHaveBeenCalledOnceWith(service.action, service.params("stub code"), service.urlParams);
  }));

  it('should call post on /runs including any task wrapping code', fakeAsync(() => {
    const promise = Promise.resolve(mockAR);
    repLoaderSpy.invoke.and.returnValue(promise);
   

    taskSubject.next({ ReadyMadeFunctions: ["codeUrl", "codeMt"], Language: '' } as ITask);

    expect(taskServiceSpy.getFile).toHaveBeenCalledOnceWith(["codeUrl", "codeMt"]);
    service.selectedLanguage = 'stub language';
    service.setFunctionDefinitions('test definitions ');
    tick();
    service.submit_run(`${UserDefinedFunctionPlaceholder}${ReadyMadeFunctionsPlaceholder}stub code`).subscribe(o => expect(o).toEqual(testRunResult));


    expect(repLoaderSpy.invoke).toHaveBeenCalledOnceWith(service.action, service.params("test definitions additional task codestub code"), service.urlParams);
  }));


  it('should call post on /runs excluding empty task wrapping code', fakeAsync(() => {
    const promise = Promise.resolve(mockAR);
    repLoaderSpy.invoke.and.returnValue(promise);
    service.selectedLanguage = 'stub language';
    service.clearFunctionDefinitions();
    service.submit_run(`${UserDefinedFunctionPlaceholder}${ReadyMadeFunctionsPlaceholder}stub code`).subscribe(o => expect(o).toEqual(testRunResult));

    expect(repLoaderSpy.invoke).toHaveBeenCalledOnceWith(service.action, service.params("stub code"), service.urlParams);
  }));


  it('should call post on /runs and return an empty result on error', fakeAsync(() => {
    repLoaderSpy.invoke.and.returnValue(Promise.reject(() => { status: 404 }));
    const unknownError = errorRunResult(null);

    service.selectedLanguage = 'stub language';

    service.submit_run("stub code").subscribe(o => expect(o).toEqual(unknownError));

    expect(repLoaderSpy.invoke).toHaveBeenCalledOnceWith(service.action, service.params("stub code"), service.urlParams);
  }));
});
