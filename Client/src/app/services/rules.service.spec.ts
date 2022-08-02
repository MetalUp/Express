import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { RulesService } from './rules.service';
import rules from '../../rules.json';
import { IRules, ErrorType, Applicability } from './rules';
import { TaskService } from './task.service';
import { EmptyTask } from './task';

describe('RulesService', () => {
  let service: RulesService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let mockTaskService;

  enum language {
    csharp = 'csharp',
    python = 'python'
  }


  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get', 'post']);
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load']);
    mockTaskService = { currentTask: EmptyTask };

    TestBed.configureTestingModule({});
    service = new RulesService(httpClientSpy, mockTaskService as TaskService);
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

  //C# expressions - fails
  it('should validate csharp expression - System', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, "System.");
    expect(validated).toEqual("Use of 'System' is not permitted");
  });

  it('should validate csharp expression - Console', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, "Console.");
    expect(validated).toEqual("Use of 'Console' is not permitted");
  });

  it('should validate csharp expression - keyword', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, "something private something");
    expect(validated).toEqual("Use of the keyword 'private' is not permitted");
  });

  it('should validate csharp expression - ArrayList', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, "something ArrayList something");
    expect(validated).toEqual("Use of ArrayList is not permitted. Use a typed list such as List<int>");
  });

  it('should validate csharp expression - assignment', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, "something a = 3 something");
    expect(validated).toEqual("Use of single '=', signifying assignment, is not permitted. (To test for equality, use '==')");
  });

  it('should validate csharp expression - mutating method', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, " myList.Add(3)");
    expect(validated).toEqual("Use of 'Add', or any other method that mutates a List is not permitted");
  });

  //C# expressions - passes

  it('should validate csharp expression - simple expression', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, " (3 + 4)*5");
    expect(validated).toEqual("");
  });




  //C# functions - fails

  it('should validate csharp function - braces', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.functions, "static int Sq(int x) => {x*x};");
    expect(validated).toEqual("Functions may not include curly braces '{' or '}'");
  });

  it('should parse csharp function - not static', () => {
    const validated = service.mustMatch(language.csharp, Applicability.functions, "int Sq(int x) => x*x;");
    expect(validated).toEqual("Functions must start with the 'static' keyword");
  });

  it('should parse csharp function - no semi-colon', () => {
    const validated = service.mustMatch(language.csharp, Applicability.functions, "static int Sq(int x) => x*x");
    expect(validated).toEqual("Functions must end with the symbol ';'");
  });

  it('should parse csharp function - no fat arrow', () => {
    const validated = service.mustMatch(language.csharp, Applicability.functions, "static int Sq(int x) return x*x");
    expect(validated).toEqual("Functions must include the symbol '=>' followed by the expression to be evaluated");
  });


  //C# functions - passes
  it('should parse csharp function - simple function', () => {
    const validated = service.mustMatch(language.csharp, Applicability.functions, "static int Sq(int x) => x*x;");
    expect(validated).toEqual("");
  });

  //Python expressions - fails
  it('should validate python expression - print', () => {
    const validated = service.mustNotContain(language.python, Applicability.expressions, "something print (3) something");
    expect(validated).toEqual("Use of 'print' is not permitted");
  });

  it('should validate python expression - input', () => {
    const validated = service.mustNotContain(language.python, Applicability.expressions, "something input something");
    expect(validated).toEqual("Use of 'input' is not permitted");
  });

  it('should validate python expression - assignment', () => {
    const validated = service.mustNotContain(language.python, Applicability.expressions, "something a = 3 something");
    expect(validated).toEqual("Use of single '=', signifying assignment, is not permitted. (To test for equality, use '==')");
  });

  it('should validate python expression - mutating method', () => {
    const validated = service.mustNotContain(language.python, Applicability.expressions, "something list.reverse() something");
    expect(validated).toEqual("Use of 'reverse', or any other method that mutates a List is not permitted");
  });

  //Python expressions - passes
  it('should validate python expression - simple expression', () => {
    const validated = service.mustNotContain(language.python, Applicability.expressions, "(3+4)*5");
    expect(validated).toEqual("");
  });

  //Python parsing - fails
  it('should parse python expression - not beginning with def', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "Foo() : return 3");
    expect(parsed).toEqual("Functions must start with the 'def' keyword followed by the function name and '('");
  });

  it('should parse python expression - no colon', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def Foo() return 3");
    expect(parsed).toEqual("Functions must include the symbol ':' followed by 'return' (may be on the next line if correctly indented) and the expression to be evaluated");
  });

  it('should parse python expression - no return', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def Foo() : 3");
    expect(parsed).toEqual("Functions must include the symbol ':' followed by 'return' (may be on the next line if correctly indented) and the expression to be evaluated");
  });

  it('should parse python expression - no indentation on newline', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def Foo() :\\nreturn 3");
    expect(parsed).toEqual("Functions must include the symbol ':' followed by 'return' (may be on the next line if correctly indented) and the expression to be evaluated");
  });

  //Python parsing - passes
  it('should parse python expression - simple case', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def Foo() : return 3");
    expect(parsed).toEqual("");
  });

  it('should parse python expression - with line breaks', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def Foo() :\n return 3");
    expect(parsed).toEqual("");
  });

});
