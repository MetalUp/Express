import { Component } from '@angular/core';
import { wrapFunctions } from '../languages/language-helpers';
import { JobeServerService } from '../services/jobe-server.service';
import { EmptyRunResult, RunResult } from '../services/run-result';

@Component({
  selector: 'app-function-definition',
  templateUrl: './function-definition.component.html',
  styleUrls: ['./function-definition.component.css']
})
export class FunctionDefinitionComponent {

  constructor(private jobeServer: JobeServerService) {
    this.result = EmptyRunResult;
  }

  result: RunResult;

  compiledOK = false;

  functionDefinitions = '';

  pendingSubmit = false;

  get currentStatus() {
    return this.result.cmpinfo || this.result.stderr || (this.compiledOK ? 'Compiled OK' : ''); 
  }

  modelChanged() {
    this.result = EmptyRunResult;
    this.pendingSubmit = !!this.functionDefinitions;
    this.jobeServer.functionDefinitions = '';
  }

  onSubmit() {
    this.compiledOK = false;
    this.pendingSubmit = false;
    const code = wrapFunctions(this.jobeServer.selectedLanguage, this.functionDefinitions);
    this.jobeServer.submit_run(code).subscribe(rr => {
      this.result = rr;
      this.compiledOK = !(this.result.cmpinfo || this.result.stderr);
      if (this.compiledOK) {
        this.jobeServer.functionDefinitions = this.functionDefinitions;
      }
    });
  }
}
