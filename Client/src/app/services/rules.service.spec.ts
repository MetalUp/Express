import { HttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { RulesService } from './rules.service';
import { IRules, ErrorType, Applicability, ITaskRules } from '../models/rules';
import { TaskService } from './task.service';
import { ITask } from '../models/task';
import { Subject } from 'rxjs';

import rules from '../../rules.json';

const taskRules : ITaskRules = {
  Messages: {
    "TaskMessage1": "a task message 1",
    "TaskMessage2": "a task message 2",
  },
  CodeMustMatch: {
    "both": [
      [
        "foo",
        "Messages.TaskMessage1"
      ]
    ],
    "expressions": [],
    "functions": []
  },
  CodeMustNotContain: {
    "both": [
      [
        "Baz",
        "Messages.TaskMessage2"
      ],
      [
        "(qux)",
        "Messages.NotPermitted"
      ]
    ],
    "expressions": [],
    "functions": []
  }
}

describe('RulesService', () => {
  let service: RulesService;
  let httpClientSpy: jasmine.SpyObj<HttpClient>;
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();
  
  enum language {
    csharp = 'csharp',
    python = 'python'
  }

  beforeEach(() => {
    httpClientSpy = jasmine.createSpyObj('HttpClient', ['get', 'post']);
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], {currentTask : taskSubject});

    TestBed.configureTestingModule({});
    service = new RulesService(httpClientSpy, taskServiceSpy);
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
  it('should mustNotContain csharp expression - System', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, "System.");
    expect(validated).toEqual("Use of 'System' is not permitted");
  });

  it('should mustNotContain csharp expression - Console', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, "Console.");
    expect(validated).toEqual("Use of 'Console' is not permitted");
  });

  it('should mustNotContain csharp expression - keyword', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, "something private something");
    expect(validated).toEqual("Use of the keyword 'private' is not permitted");
  });

  it('should mustNotContain csharp expression - ArrayList', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, "something ArrayList something");
    expect(validated).toEqual("Use of ArrayList is not permitted. Use a typed list such as List<int>");
  });

  it('should mustNotContain csharp expression - assignment', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, "something a = 3 something");
    expect(validated).toEqual("Use of single '=', signifying assignment, is not permitted. (To test for equality, use '==')");
  });

  it('should mustNotContain csharp expression - mutating method', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, " myList.Add(3)");
    expect(validated).toEqual("Use of 'Add', or any other method that mutates a List is not permitted");
  });

  //C# expressions

  it('should mustNotContain csharp expression - simple expression', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, " (3 + 4)*5");
    expect(validated).toEqual("");
  });

  it('should mustNotContain csharp expression - expression with ;', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.expressions, " (3 + 4)*5;");
    expect(validated).toEqual("Use of ';' is not permitted");
  });

    //C# functions - passes
  it('should mustMatch csharp function - simple function', () => {
    const validated = service.mustMatch(language.csharp, Applicability.functions, "static int Sq(int x) => x*x;");
    expect(validated).toEqual("");
  });

  it('should mustMatch csharp function - multiple functions', () => {
    const validated = service.mustMatch(language.csharp, Applicability.functions, "static int Sq(int x) => x*x;\nstatic int Foo(int x) => 3;");
    expect(validated).toEqual("");
  });
  it('should mustMatch csharp function - function over multiple lines', () => {
    const validated = service.mustMatch(language.csharp, Applicability.functions, "static int Sq(int x) \n =>\n x*\nx\n;");
    expect(validated).toEqual("");
  });
  it('should mustMatch csharp function - complex example', () => {
    const validated = service.mustMatch(language.csharp, Applicability.functions, "static List<int> NeighbourCells(int c) => new List<int> { c - 21, c - 20, c - 19, c - 1, c + 1, c + 19, c + 20, c + 21 };\n\nstatic int KeepWithinBounds(int i) => (i + 400) % 400;\n\nstatic List<int> AdjustedNeighbourCells(int c) => NeighbourCells(c).Select(x => KeepWithinBounds(x)).ToList();\nstatic int LiveNeighbours(List<bool> cells, int c) => AdjustedNeighbourCells(c).Where(i => cells[i] == true).Count();\nstatic bool WillLive(bool currentlyAlive, int liveNeighbours) => (currentlyAlive ? liveNeighbours > 1 && liveNeighbours < 4 : liveNeighbours == 3);\n \nstatic bool NextCellValue(List<bool> cells, int c) => WillLive(cells[c], LiveNeighbours(cells, c));\nstatic List<bool> NextGeneration(List<bool> cells) => Enumerable.Range(0, 400).Select(n => NextCellValue(cells, n)).ToList();");
    expect(validated).toEqual("");
  });
  //C# functions - fails

  it('should mustMatch csharp function - not static', () => {
    const validated = service.mustMatch(language.csharp, Applicability.functions, "int Sq(int x) => x*x;");
    expect(validated).toEqual("Functions must start with 'static'");
  });

  it('should mustNotContain csharp function - braces', () => {
    const validated = service.mustNotContain(language.csharp, Applicability.functions, "static int Sq(int x) => {x*x};");
    expect(validated).toEqual("Function implementation may not start with curly brace '{'");
  });

  it('should mustMatch csharp function - no fat arrow', () => {
    const validated = service.mustMatch(language.csharp, Applicability.functions, "static int Sq(int x) return x*x");
    expect(validated).toEqual("Functions must include the symbol '=>'");
  });

  //Python expressions - fails
  it('should mustNotContain python expression - print', () => {
    const validated = service.mustNotContain(language.python, Applicability.expressions, "something print (3) something");
    expect(validated).toEqual("Use of 'print' is not permitted");
  });

  it('should mustNotContain python expression - input', () => {
    const validated = service.mustNotContain(language.python, Applicability.expressions, "something input() something");
    expect(validated).toEqual("Use of 'input' is not permitted");
  });

  it('should mustNotContain python expression - assignment', () => {
    const validated = service.mustNotContain(language.python, Applicability.expressions, "something a = 3 something");
    expect(validated).toEqual("Use of single '=', signifying assignment, is not permitted. (To test for equality, use '==')");
  });

  it('should mustNotContain python expression - mutating method', () => {
    const validated = service.mustNotContain(language.python, Applicability.expressions, "something list.reverse() something");
    expect(validated).toEqual("Use of 'reverse', or any other method that mutates a List is not permitted");
  });

  //Python expressions - passes
  it('should mustNotContain python expression - simple expression', () => {
    const validated = service.mustNotContain(language.python, Applicability.expressions, "(3+4)*5");
    expect(validated).toEqual("");
  });

  it('should mustNotContain python expression - semi-colon', () => {
    const validated = service.mustNotContain(language.python, Applicability.expressions, "3+4;5");
    expect(validated).toEqual("Use of ';' is not permitted");
  });

  //Python parsing - fails
  it('should mustMatch python expression - not beginning with def', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "Foo() : return 3");
    expect(parsed).toEqual("Function definitions must start with 'def '");
  });

  it('should mustMatch python expression - no colon', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def foo() return 3");
    expect(parsed).toEqual("Function signature should be followed by ':' (with or without a space) on the same line");
  });

  it('should mustMatch python expression - no return', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def foo() : 3");
    expect(parsed).toEqual("The ':' should be followed by 'return' on the same line, or on the next line but indented");
  });

  it('should mustMatch python expression - no semi colon', () => {
    const parsed = service.mustNotContain(language.python, Applicability.functions, "def foo() : return 3; 4");
    expect(parsed).toEqual("Use of ';' is not permitted");
  });

  it('should mustMatch python expression - function may not be formatted over muliple lines without backslash', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def foo() : return 3 + \n4");
    expect(parsed).toEqual("All functions must follow the standard form: def <name>(<parameters>): return <expression>");
  });

  //Python parsing - passes
  it('should mustMatch python expression - simple case', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def foo() : return 3");
    expect(parsed).toEqual("");
  });

  it('should mustMatch python expression - without spaces around :', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def foo():return 3");
    expect(parsed).toEqual("");
  });

  it('should mustMatch python expression - with line breaks', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def foo() :\n return 3");
    expect(parsed).toEqual("");
  });

  it('should mustMatch python expression - multiple function defs', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def foo() :\n return 3\ndef bar() : return 4");
    expect(parsed).toEqual("");
  });

  it('should mustMatch python expression - function may be formatted over muliple lines using backslash', () => {
    const parsed = service.mustMatch(language.python, Applicability.functions, "def foo() : return 3 + \\n4");
    expect(parsed).toEqual("");
  });

  // task rules

  it('should pick up mustMatch rules from the task - pass', () => {
    taskSubject.next(taskRules as ITask);

    const parsed = service.mustMatch(language.python, Applicability.functions, "def foo() :\n return 3");
    expect(parsed).toEqual("");
  });

  it('should pick up mustMatch rules from the task - fail', () => {
    taskSubject.next(taskRules as ITask);

    const parsed = service.mustMatch(language.python, Applicability.functions, "def baz() :\n return 3");
    expect(parsed).toEqual("a task message 1");
  });

  it('should pick up mustNotContain rules from the task - pass', () => {
    taskSubject.next(taskRules as ITask);

    const parsed = service.mustNotContain(language.python, Applicability.functions, "def foo() :\n return Bar");
    expect(parsed).toEqual("");
  });

  it('should pick up mustNotContain rules from the task - fail', () => {
    taskSubject.next(taskRules as ITask);

    const parsed = service.mustNotContain(language.python, Applicability.functions, "def foo() :\n return Baz");
    expect(parsed).toEqual("a task message 2");
  });

  it('should pick up messages from the rules.json file', () => {
    taskSubject.next(taskRules as ITask);

    const parsed = service.mustNotContain(language.python, Applicability.functions, "def foo() :\n return qux");
    expect(parsed).toEqual("Use of 'qux' is not permitted");
  });

});
