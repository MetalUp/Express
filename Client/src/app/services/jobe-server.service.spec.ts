import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { JobeServerService } from './jobe-server.service';
import { errorRunResult, RunResult } from './run-result';
import { of, Subject, throwError } from 'rxjs';
import { FunctionPlaceholder, ReadyMadeFunctionsPlaceholder } from '../languages/language-helpers';
import { TaskService } from './task.service';
import { ITask } from './task';

describe('JobeServerService', () => {
  let service: JobeServerService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();

  let testRunSpec: { "run_spec": { "language_id": string, "sourcecode": string } };
  let testlanguages: [string, string][] = [["l1", "v1"]];
  let testRunResult: RunResult = {
    run_id: 'a',
    outcome: 66,
    cmpinfo: 'b',
    stdout: 'c',
    stderr: 'd'
  }

  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get', 'post']);
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], { currentTask: taskSubject });

    TestBed.configureTestingModule({});
    service = new JobeServerService(httpClientSpy, taskServiceSpy);
    service.selectedLanguage = 'stub language';
    testRunSpec = service.run_spec('stub code');
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call post on /runs', () => {
    httpClientSpy.post.and.returnValue(of<RunResult>(testRunResult));
    service.selectedLanguage = 'stub language';
    service.submit_run("stub code").subscribe(o => expect(o).toEqual(testRunResult));

    expect(httpClientSpy.post).toHaveBeenCalledOnceWith(`${service.path}/runs`,
      Object(testRunSpec),
      jasmine.any(Object));
  });

  it('should call post on /runs including any function definitions', () => {
    httpClientSpy.post.and.returnValue(of<RunResult>(testRunResult));
    service.selectedLanguage = 'stub language';
    service.setFunctionDefinitions('test definitions ');

    service.submit_run(`${FunctionPlaceholder}stub code`).subscribe(o => expect(o).toEqual(testRunResult));

    testRunSpec.run_spec.sourcecode = 'test definitions stub code';

    expect(httpClientSpy.post).toHaveBeenCalledOnceWith(`${service.path}/runs`,
      Object(testRunSpec),
      jasmine.any(Object));
  });

  it('should call post on /runs excluding empty function definitions', () => {
    httpClientSpy.post.and.returnValue(of<RunResult>(testRunResult));
    service.selectedLanguage = 'stub language';
    service.clearFunctionDefinitions();

    service.submit_run(`${FunctionPlaceholder}stub code`).subscribe(o => expect(o).toEqual(testRunResult));

    expect(httpClientSpy.post).toHaveBeenCalledOnceWith(`${service.path}/runs`,
      Object(testRunSpec),
      jasmine.any(Object));
  });

  it('should call post on /runs including any task wrapping code', () => {
    httpClientSpy.post.and.returnValue(of<RunResult>(testRunResult));
    taskSubject.next({ WrappingCode: "additional task code " } as ITask);

    service.selectedLanguage = 'stub language';
    service.setFunctionDefinitions('test definitions ');

    service.submit_run(`${FunctionPlaceholder}${ReadyMadeFunctionsPlaceholder}stub code`).subscribe(o => expect(o).toEqual(testRunResult));

    testRunSpec.run_spec.sourcecode = 'test definitions additional task code stub code';

    expect(httpClientSpy.post).toHaveBeenCalledOnceWith(`${service.path}/runs`,
      Object(testRunSpec),
      jasmine.any(Object));
  });

  it('should call post on /runs excluding empty task wrapping code', () => {
    httpClientSpy.post.and.returnValue(of<RunResult>(testRunResult));
    service.selectedLanguage = 'stub language';
    service.clearFunctionDefinitions();

    service.submit_run(`${FunctionPlaceholder}${ReadyMadeFunctionsPlaceholder}stub code`).subscribe(o => expect(o).toEqual(testRunResult));

    expect(httpClientSpy.post).toHaveBeenCalledOnceWith(`${service.path}/runs`,
      Object(testRunSpec),
      jasmine.any(Object));
  });


  it('should call post on /runs and return an empty result on error', () => {
    httpClientSpy.post.and.returnValue(throwError(() => { status: 404 }));
    const unknownError = errorRunResult(null);

    service.selectedLanguage = 'stub language';
    service.submit_run("stub code").subscribe(o => expect(o).toEqual(unknownError));

    expect(httpClientSpy.post).toHaveBeenCalledOnceWith(`${service.path}/runs`,
      Object(testRunSpec),
      jasmine.any(Object));
  });

  it('should call get on /languages', () => {
    httpClientSpy.get.and.returnValue(of<[string, string][]>(testlanguages));

    service.get_languages().subscribe(o => expect(o).toEqual(testlanguages));

    expect(httpClientSpy.get).toHaveBeenCalledOnceWith(`${service.path}/languages`, jasmine.any(Object));
  });

  it('should call get on /languages and return empty array on error', () => {
    httpClientSpy.get.and.returnValue(throwError(() => { status: 404 }));

    service.get_languages().subscribe(o => expect(o).toEqual([]));

    expect(httpClientSpy.get).toHaveBeenCalledOnceWith(`${service.path}/languages`, jasmine.any(Object));
  });
});
