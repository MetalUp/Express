import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { TaskService } from './task.service';
import { of } from 'rxjs';

describe('TaskService', () => {
  let service: TaskService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;
  let routerSpy: jasmine.SpyObj<Router>;

  let testParams: any = {};

  let params = { queryParams: of<Params>(testParams) }

  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get', 'post']);
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    TestBed.configureTestingModule({});
    service = new TaskService(httpClientSpy, routerSpy, params as ActivatedRoute)
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
