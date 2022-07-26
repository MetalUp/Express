import { Component } from '@angular/core';
import { filterCmpinfo, filterStderr, wrapFunctions } from '../languages/language-helpers';
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

  submitting = false;

  get currentStatus() {
    return  filterCmpinfo(this.jobeServer.selectedLanguage, this.result.cmpinfo) ||
            filterStderr(this.jobeServer.selectedLanguage, this.result.stderr) ||
            (this.compiledOK ? 'Compiled OK' : ''); 
  }

  modelChanged() {
    this.compiledOK = false;
    this.result = EmptyRunResult;
    this.pendingSubmit = !!this.functionDefinitions;
    this.jobeServer.clearFunctionDefinitions();
  }

  onSubmit() {
    this.submitting = true;
    this.compiledOK = false;
    this.pendingSubmit = false;
    const code = wrapFunctions(this.jobeServer.selectedLanguage, this.functionDefinitions);
    this.jobeServer.submit_run(code).subscribe(rr => {
      this.result = rr;
      this.compiledOK = !(this.result.cmpinfo || this.result.stderr) && this.result.outcome == 15;
      if (this.compiledOK) {
        this.jobeServer.setFunctionDefinitions(this.functionDefinitions);
      }
      this.submitting = false;
    });
  }
}
