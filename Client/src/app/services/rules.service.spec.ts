import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { Applicability, ErrorType, RulesService } from './rules.service';
import rules from '../../rules.json';

describe('RulesService', () => {
  let service: RulesService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;
 

  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get', 'post']);
    TestBed.configureTestingModule({});
    service = new RulesService(httpClientSpy);

    service.rules = rules as any;
  
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should filter csharp cmpinfo', () => {
    const filtered = service.filter('csharp', ErrorType.cmpinfo, "something CSXXXX something else");
    expect(filtered).toEqual('CSXXXX something else')
  });
 
  it('should filter csharp stderr', () => {
    const filtered = service.filter('csharp', ErrorType.stderr, "something NullException something else");
    expect(filtered).toEqual('NullException')
  });

  it('should filter python cmpinfo', () => {
    const filtered = service.filter('python', ErrorType.cmpinfo, "something Error: something else");
    expect(filtered).toEqual(' Error: something else')
  });
 
  it('should filter python stderr', () => {
    const filtered = service.filter('python', ErrorType.stderr, "something Error: something else");
    expect(filtered).toEqual(' Error: something else')
  });


  it('should validate csharp expression - System', () => {
    const validated = service.validate('csharp', Applicability.expressions, "System.");
    expect(validated).toEqual("Explicit use of the 'System' namespace is unnecessary, and not permitted");
  });

  it('should validate csharp expression - Console', () => {
    const validated = service.validate('csharp', Applicability.expressions, "Console.");
    expect(validated).toEqual("Use of  Console. is not permitted in MetalUp.Express");
  });



});
