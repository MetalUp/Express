import { TestBed } from '@angular/core/testing';
import { RulesService } from './rules.service';
import { ErrorType, Applicability } from '../models/rules';
import { TaskService } from './task.service';
import { ITask } from '../models/task';
import { Subject } from 'rxjs';


describe('RulesService', () => {
  let service: RulesService;
 
  let taskServiceSpy: jasmine.SpyObj<TaskService>;
  let taskSubject = new Subject<ITask>();

  let regex = 
  {
    "Messages": {
        "DisallowedKeyword": "Use of the keyword '{1}' is not permitted",
        "NotPermitted": "Use of '{1}' is not permitted",
        "ExternalDependency": "Use of '{1}' is not permitted here, as this would make the function dependent on a variable not passed in as a parameter",
        "MutatingMethod": "Use of '{1}', or any other method that mutates a List is not permitted",
        "Assignment": "Use of single '=', signifying assignment, is not permitted. (To test for equality, use '==')"
    },
    "ServerResponseMessageFilters": {
        "cmpinfo": "CS.*",
        "stderr": "\\w+Exception"
    },
    "CodeMustMatch": {
        "both": [],
        "expressions": [],
        "functions": [
            [
                "^\\s*static\\s+.*",
                "Functions must start with 'static'"
            ],
            [
                "^(?:.|\\n)*=>.*",
                "Functions must include the symbol '=>'"
            ],
            [
                "^(?:static\\s(?:.|\\n)*=>(?:.|\\n)*;\\s*)*$",
                "Functions must follow the form: static <ReturnType> <NameStartingInUpperCase>(<parameters>) => <expression>;"
            ]
        ]
    },
    "CodeMustNotContain": {
        "both": [
            [
                ".*(?:^|\\s+)(return|var|void|using|public|private|protected|class|abstract|readOnly)\\s.*",
                "Messages.DisallowedKeyword"
            ],
            [
                "(Console|System)\\.",
                "Messages.NotPermitted"
            ],
            [
                "\\w*[^(=|>|<)]=[^(=|>)]\\w*",
                "Messages.Assignment"
            ],
            [
                "\\W(ArrayList)\\W",
                "Use of ArrayList is not permitted. Use a typed list such as List<int>"
            ],
            [
                "\\.(Add|AddRange|Clear|RemoveAll|RemoveAt|RemoveRange)\\s*\\(",
                "Messages.MutatingMethod"
            ]
        ],
        "expressions": [
            [
                "(;)",
                "Messages.NotPermitted"
            ]
        ],
        "functions": [
            [
                "\\s+DateTime\\.(Today|Now)",
                "Messages.ExternalDependency"
            ],
            [
                "=>\\s*{",
                "Function implementation may not start with curly brace '{'"
            ]
        ]
    }
};



  beforeEach(() => {
    taskServiceSpy = jasmine.createSpyObj('TaskService', ['load'], {currentTask : taskSubject});

    TestBed.configureTestingModule({});
    service = new RulesService(taskServiceSpy);
    taskSubject.next( {RegExRules : JSON.stringify(regex)} as ITask);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should filter cmpinfo', () => {
    const filtered = service.filter(ErrorType.cmpinfo, "something CSXXXX something else");
    expect(filtered).toEqual('CSXXXX something else')
  });

  it('should filter stderr', () => {
    const filtered = service.filter(ErrorType.stderr, "something NullException something else");
    expect(filtered).toEqual('NullException')
  });

  //expressions - fails
  it('should mustNotContain expression - System', () => {
    const validated = service.mustNotContain(Applicability.expressions, "System.");
    expect(validated).toEqual("Use of 'System' is not permitted");
  });

  it('should mustNotContain expression - Console', () => {
    const validated = service.mustNotContain(Applicability.expressions, "Console.");
    expect(validated).toEqual("Use of 'Console' is not permitted");
  });

  it('should mustNotContain expression - keyword', () => {
    const validated = service.mustNotContain(Applicability.expressions, "something private something");
    expect(validated).toEqual("Use of the keyword 'private' is not permitted");
  });

  it('should mustNotContain expression - ArrayList', () => {
    const validated = service.mustNotContain(Applicability.expressions, "something ArrayList something");
    expect(validated).toEqual("Use of ArrayList is not permitted. Use a typed list such as List<int>");
  });

  it('should mustNotContain expression - assignment', () => {
    const validated = service.mustNotContain(Applicability.expressions, "something a = 3 something");
    expect(validated).toEqual("Use of single '=', signifying assignment, is not permitted. (To test for equality, use '==')");
  });

  it('should mustNotContain expression - mutating method', () => {
    const validated = service.mustNotContain(Applicability.expressions, " myList.Add(3)");
    expect(validated).toEqual("Use of 'Add', or any other method that mutates a List is not permitted");
  });

  //C# expressions

  it('should mustNotContain expression - simple expression', () => {
    const validated = service.mustNotContain(Applicability.expressions, " (3 + 4)*5");
    expect(validated).toEqual("");
  });

  it('should mustNotContain expression - expression with ;', () => {
    const validated = service.mustNotContain(Applicability.expressions, " (3 + 4)*5;");
    expect(validated).toEqual("Use of ';' is not permitted");
  });

  //C# functions - passes
  it('should mustMatch function - simple function', () => {
    const validated = service.mustMatch(Applicability.functions, "static int Sq(int x) => x*x;");
    expect(validated).toEqual("");
  });

  it('should mustMatch function - multiple functions', () => {
    const validated = service.mustMatch(Applicability.functions, "static int Sq(int x) => x*x;\nstatic int Foo(int x) => 3;");
    expect(validated).toEqual("");
  });

  it('should mustMatch function - function over multiple lines', () => {
    const validated = service.mustMatch(Applicability.functions, "static int Sq(int x) \n =>\n x*\nx\n;");
    expect(validated).toEqual("");
  });

  it('should mustMatch function - complex example', () => {
    const validated = service.mustMatch(Applicability.functions, "static List<int> NeighbourCells(int c) => new List<int> { c - 21, c - 20, c - 19, c - 1, c + 1, c + 19, c + 20, c + 21 };\n\nstatic int KeepWithinBounds(int i) => (i + 400) % 400;\n\nstatic List<int> AdjustedNeighbourCells(int c) => NeighbourCells(c).Select(x => KeepWithinBounds(x)).ToList();\nstatic int LiveNeighbours(List<bool> cells, int c) => AdjustedNeighbourCells(c).Where(i => cells[i] == true).Count();\nstatic bool WillLive(bool currentlyAlive, int liveNeighbours) => (currentlyAlive ? liveNeighbours > 1 && liveNeighbours < 4 : liveNeighbours == 3);\n \nstatic bool NextCellValue(List<bool> cells, int c) => WillLive(cells[c], LiveNeighbours(cells, c));\nstatic List<bool> NextGeneration(List<bool> cells) => Enumerable.Range(0, 400).Select(n => NextCellValue(cells, n)).ToList();");
    expect(validated).toEqual("");
  });
  //functions - fails

  it('should mustMatch function - not static', () => {
    const validated = service.mustMatch(Applicability.functions, "int Sq(int x) => x*x;");
    expect(validated).toEqual("Functions must start with 'static'");
  });

  it('should mustNotContain function - braces', () => {
    const validated = service.mustNotContain(Applicability.functions, "static int Sq(int x) => {x*x};");
    expect(validated).toEqual("Function implementation may not start with curly brace '{'");
  });

  it('should mustMatch function - no fat arrow', () => {
    const validated = service.mustMatch(Applicability.functions, "static int Sq(int x) return x*x");
    expect(validated).toEqual("Functions must include the symbol '=>'");
  });
});
