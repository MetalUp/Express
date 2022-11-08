import { TestBed } from '@angular/core/testing';

import { RegisteredService } from './registered.service';

describe('RegisteredService', () => {
  let service: RegisteredService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RegisteredService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
