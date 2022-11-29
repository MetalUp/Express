import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Subject } from 'rxjs';
import { RunResult } from '../models/run-result';
import { CompileServerService } from '../services/compile-server.service';

import { ResultComponent } from './result.component';

describe('ResultComponent', () => {
  let component: ResultComponent;
  let fixture: ComponentFixture<ResultComponent>;
  let compileServerServiceSpy: jasmine.SpyObj<CompileServerService>;
  let resultSubject = new Subject<RunResult>();


  beforeEach(async () => {
    compileServerServiceSpy = jasmine.createSpyObj('CompileServerService', ['evaluateExpression'], { "selectedLanguage": "csharp",  lastExpressionResult: resultSubject  });
  
    
    await TestBed.configureTestingModule({
      declarations: [ResultComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {
          provide: CompileServerService,
          useValue: compileServerServiceSpy
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
