import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { JobeServerService } from './jobe-server.service';
import { RunResult } from './run-result';
import { of, throwError } from 'rxjs';

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
    //service = TestBed.inject(JobeServerService);
    service = new JobeServerService(httpClientSpy);
    testRunSpec = service.run_spec('stub language', 'stub code');
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call post on /runs', () => {
    httpClientSpy.post.and.returnValue(of<RunResult>(testRunResult));

    service.submit_run("stub language", "stub code").subscribe(o => expect(o).toEqual(testRunResult));

    expect(httpClientSpy.post).toHaveBeenCalledOnceWith(`${service.path}/runs`,
      Object(testRunSpec),
      jasmine.any(Object));
  });

  it('should call post on /runs and return an empty result on error', () => {
    httpClientSpy.post.and.returnValue(throwError(() => { status: 404 }));

    service.submit_run("stub language", "stub code").subscribe(o => expect(o).toEqual(service.emptyResult));

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
