import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { TaskService } from './task.service';
import { of, Subject } from 'rxjs';
import { ITask } from './task';
import { first } from 'rxjs';

describe('TaskService', () => {
  let service: TaskService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;
  let routerSpy: jasmine.SpyObj<Router>;

  let testParams = new Subject<Params>();
  let params = { queryParams: testParams } as unknown as ActivatedRoute;

  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get', 'post']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    httpClientSpy.get.and.returnValue(of({ Language: 'testlanguage' } as unknown as ITask))

    TestBed.configureTestingModule({});
    service = new TaskService(httpClientSpy, routerSpy, params)
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get the task', () => {
    service.currentTask.pipe(first()).subscribe(t =>
      expect(t.Language).toEqual('testlanguage')
    );

    testParams.next({ task: 'testTask' });

    expect(httpClientSpy.get).toHaveBeenCalledWith('content/testTask.json', { withCredentials: true });
  });

  it('should get the html file for the task', () => {

    service.getHtml('testHtmlFile.html');

    const parms = { withCredentials: true, responseType: 'text' as const };

    expect(httpClientSpy.get).toHaveBeenCalledWith('content/testHtmlFile.html', parms as any);
  });

  it('should goto a new task', () => {
    service.currentTask.pipe(first()).subscribe(t =>
      expect(t.Language).toEqual('testlanguage')
    );

    service.gotoTask('newTask.json');

    expect(routerSpy.navigate).toHaveBeenCalledWith(['/'], { queryParams: { task: 'newTask' } });
  });
});
