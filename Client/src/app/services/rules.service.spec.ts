import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { Applicability, ErrorType, IRules, RulesService } from './rules.service';
import rules from '../../rules.json';

describe('RulesService', () => {
  let service: RulesService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;
 
  enum language{
    csharp = 'csharp',
    python = 'python'
  }


  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get', 'post']);
    TestBed.configureTestingModule({});
    service = new RulesService(httpClientSpy);
    service.rules = rules as unknown as IRules;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should filter csharp cmpinfo', () => {
    const filtered = service.filter(language.csharp, ErrorType.cmpinfo, "something CSXXXX something else");
    expect(filtered).toEqual('CSXXXX something else')
  });
 
  it('should filter csharp stderr', () => {
    const filtered = service.filter(language.csharp, ErrorType.stderr, "something NullException something else");
    expect(filtered).toEqual('NullException')
  });

  it('should filter python cmpinfo', () => {
    const filtered = service.filter(language.python, ErrorType.cmpinfo, "something Error: something else");
    expect(filtered).toEqual(' Error: something else')
  });
 
  it('should filter python stderr', () => {
    const filtered = service.filter(language.python, ErrorType.stderr, "something Error: something else");
    expect(filtered).toEqual(' Error: something else')
  });

  //C# expressions
  it('should validate csharp expression - System', () => {
    const validated = service.validate(language.csharp, Applicability.expressions, "System.");
    expect(validated).toEqual("Use of 'System' is not permitted");
  });

  it('should validate csharp expression - Console', () => {
    const validated = service.validate(language.csharp, Applicability.expressions, "Console.");
    expect(validated).toEqual("Use of 'Console' is not permitted");
  });

  //Python expressions
  it('should validate python expression - print', () => {
    const validated = service.validate(language.python, Applicability.expressions, "something print (3) something");
    expect(validated).toEqual("Use of 'print' is not permitted");
  });

  it('should validate python expression - input', () => {
    const validated = service.validate(language.python, Applicability.expressions, "something input something");
    expect(validated).toEqual("Use of 'input' is not permitted");
  });

  it('should validate python expression - assignment', () => {
    const validated = service.validate(language.python, Applicability.expressions, "something a = 3 something");
    expect(validated).toEqual("Use of single '=', signifying assignment, is not permitted. (To test for equality, use '==')");
  });

  //Python parsing
  it('should parse python expression - not beginning with def', () => {
    const parsed = service.parse(language.python, Applicability.functions, "Foo() : return 3");
    expect(parsed).toEqual("Functions must start with the 'def' keyword followed by the function name and '('");
  });

});
