import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';

import { JobeServerService } from './jobe-server.service';

describe('JobeServerService', () => {
  let service: JobeServerService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;

  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
    TestBed.configureTestingModule({});
    //service = TestBed.inject(JobeServerService);
    service = new JobeServerService(httpClientSpy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
