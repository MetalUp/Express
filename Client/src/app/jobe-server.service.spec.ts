import { TestBed } from '@angular/core/testing';

import { JobeServerService } from './jobe-server.service';

describe('JobeServerService', () => {
  let service: JobeServerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(JobeServerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
