import { Component } from '@angular/core';
import { wrapFunctions } from '../languages/language-helpers';
import { JobeServerService } from '../services/jobe-server.service';
import { Applicability, ErrorType } from '../services/rules';
import { RulesService } from '../services/rules.service';
import { EmptyRunResult, RunResult } from '../services/run-result';

@Component({
  selector: 'app-function-definition',
  templateUrl: './function-definition.component.html',
  styleUrls: ['./function-definition.component.css']
})
export class FunctionDefinitionComponent {

  constructor(private jobeServer: JobeServerService, private rulesService: RulesService) {
    this.result = EmptyRunResult;
  }

  result: RunResult;

  compiledOK = false;

  functionDefinitions = '';

  pendingSubmit = false;

  submitting = false;

  validationFail: string = '';

  get currentStatus() {
    return this.validationFail ||
      this.rulesService.filter(this.jobeServer.selectedLanguage, ErrorType.cmpinfo, this.result.cmpinfo) ||
      this.rulesService.filter(this.jobeServer.selectedLanguage, ErrorType.stderr, this.result.stderr) ||
      (this.compiledOK ? 'Compiled OK' : '');
  }

  modelChanged() {
    this.validationFail = '';
    this.compiledOK = false;
    this.result = EmptyRunResult;
    this.pendingSubmit = !!(this.functionDefinitions.trim());
    this.jobeServer.clearFunctionDefinitions();
  }

  onSubmit() {
    this.compiledOK = false;
    this.pendingSubmit = false;
    this.validationFail = this.rulesService.parse(this.jobeServer.selectedLanguage, Applicability.functions, this.functionDefinitions) ||
      this.rulesService.validate(this.jobeServer.selectedLanguage, Applicability.functions, this.functionDefinitions);

    if (!this.validationFail) {
      this.submitting = true;
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
}
