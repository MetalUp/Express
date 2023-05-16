import { TestBed } from '@angular/core/testing';

import { AppVersionHttpInterceptor } from './app-version.http-interceptor';

describe('AppVersionHttpInterceptorInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      AppVersionHttpInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: AppVersionHttpInterceptor = TestBed.inject(AppVersionHttpInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
