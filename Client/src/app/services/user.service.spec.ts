import { TestBed } from '@angular/core/testing';
import { ContextService, RepLoaderService } from '@nakedobjects/services';

import { UserService } from './user.service';

describe('UserService', () => {
  let service: UserService;
  let contextServiceSpy: jasmine.SpyObj<ContextService>;
  let repLoaderSpy: jasmine.SpyObj<RepLoaderService>;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = new UserService(contextServiceSpy, repLoaderSpy);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
