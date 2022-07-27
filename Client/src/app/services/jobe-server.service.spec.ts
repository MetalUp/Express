import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { JobeServerService } from './jobe-server.service';
import { EmptyRunResult, errorRunResult, RunResult } from './run-result';
import { of, throwError } from 'rxjs';
import { FunctionPlaceholder } from '../languages/language-helpers';

describe('JobeServerService', () => {
  let service: JobeServerService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;
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
    TestBed.configureTestingModule({});
    service = new JobeServerService(httpClientSpy);
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
