import { ComponentFixture, TestBed } from '@angular/core/testing';
import { wrapFunctions } from '../languages/language-helpers';
import { JobeServerService } from '../services/jobe-server.service';
import { RunResult } from '../services/run-result';
import { of } from 'rxjs';

import { FunctionDefinitionComponent } from './function-definition.component';
import { RulesService } from '../services/rules.service';

describe('FunctionDefinitionComponent', () => {
  let component: FunctionDefinitionComponent;
  let fixture: ComponentFixture<FunctionDefinitionComponent>;
  let mockJobeServerService: jasmine.SpyObj<JobeServerService>;
  let mockRulesService: jasmine.SpyObj<RulesService>;

  let testRunResultOK: RunResult = {
    run_id: 'a',
    outcome: 15,
    cmpinfo: '',
    stdout: 'expression result',
    stderr: ''
  }

  let testRunResultCmp: RunResult = {
    run_id: 'a',
    outcome: 11,
    cmpinfo: 'compiler error',
    stdout: '',
    stderr: ''
  }

  let testRunResultErr: RunResult = {
    run_id: 'a',
    outcome: 12,
    cmpinfo: '',
    stdout: '',
    stderr: 'run error'
  }


  beforeEach(async () => {
    mockJobeServerService = jasmine.createSpyObj('JobeServerService', ['submit_run', 'clearFunctionDefinitions', 'setFunctionDefinitions'], {"selectedLanguage":"csharp"});
    mockRulesService = jasmine.createSpyObj('RulesService', ['filter', 'validate', 'parse']);

    await TestBed.configureTestingModule({
      declarations: [ FunctionDefinitionComponent ],
      providers: [
        {
          provide: JobeServerService,
          useValue: mockJobeServerService
        },
        {
          provide: RulesService,
          useValue: mockRulesService
        }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FunctionDefinitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });


  it('should submit code for compile OK', () => {
    mockRulesService.parse.and.returnValue('');
    mockRulesService.validate.and.returnValue('');
    mockJobeServerService.submit_run.and.returnValue(of<RunResult>(testRunResultOK));

    component.functionDefinitions = 'test';
    const wrapped = wrapFunctions('csharp', component.functionDefinitions);

    component.onSubmit();
    expect(mockJobeServerService.submit_run).toHaveBeenCalledWith(wrapped);
    
    expect(component.compiledOK).toBe(true);
    expect(component.currentStatus).toBe('Compiled OK');
    expect(component.pendingSubmit).toBe(false);
  
    expect(mockJobeServerService.setFunctionDefinitions).toHaveBeenCalledWith(component.functionDefinitions);

  });

  it('should submit code for compile Fail', () => {
    mockRulesService.parse.and.returnValue('');
    mockRulesService.validate.and.returnValue('');
    mockRulesService.filter.and.returnValue('compiler error');
    mockJobeServerService.submit_run.and.returnValue(of<RunResult>(testRunResultCmp));

    component.functionDefinitions = 'test';
    const wrapped = wrapFunctions('csharp', component.functionDefinitions);

    component.onSubmit();
    expect(mockJobeServerService.submit_run).toHaveBeenCalledWith(wrapped);
    
    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('compiler error');
    expect(component.pendingSubmit).toBe(false);
  
    expect(mockJobeServerService.setFunctionDefinitions).not.toHaveBeenCalledWith(component.functionDefinitions);

  });

  it('should submit code for compile Error', () => {
    mockRulesService.parse.and.returnValue('');
    mockRulesService.validate.and.returnValue('');
    mockRulesService.filter.and.returnValue('run error');
    mockJobeServerService.submit_run.and.returnValue(of<RunResult>(testRunResultErr));

    component.functionDefinitions = 'test';
    const wrapped = wrapFunctions('csharp', component.functionDefinitions);

    component.onSubmit();
    expect(mockJobeServerService.submit_run).toHaveBeenCalledWith(wrapped);
    
    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('run error');
    expect(component.pendingSubmit).toBe(false);
  
    expect(mockJobeServerService.setFunctionDefinitions).not.toHaveBeenCalledWith(component.functionDefinitions);

  });


  it('should clear code when changed', () => {
    component.functionDefinitions = 'test';
    component.modelChanged();
       
    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(true);
  
    expect(mockJobeServerService.clearFunctionDefinitions).toHaveBeenCalled();
  });

  it('should not allow empty code to be submitted', () => {
    
    component.modelChanged();
       
    expect(component.compiledOK).toBe(false);
    expect(component.currentStatus).toBe('');
    expect(component.pendingSubmit).toBe(false);
  
    expect(mockJobeServerService.clearFunctionDefinitions).toHaveBeenCalled();
  });

});
