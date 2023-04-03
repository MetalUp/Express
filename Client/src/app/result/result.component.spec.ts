import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Subject } from 'rxjs';
import { RunResult } from '../models/run-result';
import { CompileServerService } from '../services/compile-server.service';
import { RulesService } from '../services/rules.service';

import { ResultComponent } from './result.component';

describe('ResultComponent', () => {
  let component: ResultComponent;
  let fixture: ComponentFixture<ResultComponent>;
  let compileServerServiceSpy: jasmine.SpyObj<CompileServerService>;
  let rulesServiceSpy: jasmine.SpyObj<RulesService>;
  const resultSubject = new Subject<RunResult>();


  beforeEach(async () => {
    compileServerServiceSpy = jasmine.createSpyObj('CompileServerService', ['evaluateExpression'], { "selectedLanguage": "csharp",  lastExpressionResult: resultSubject  });
    rulesServiceSpy = jasmine.createSpyObj('RulesService', ['filter', 'checkRules']);
    rulesServiceSpy.checkRules.and.returnValue('');
    rulesServiceSpy.filter.and.callFake(( _e, tf) => tf);
    
    await TestBed.configureTestingModule({
      declarations: [ResultComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: CompileServerService,
          useValue: compileServerServiceSpy
        },
        {
          provide: RulesService,
          useValue: rulesServiceSpy
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
