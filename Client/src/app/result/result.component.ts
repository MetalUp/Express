import { Component, OnInit } from '@angular/core';
import { ErrorType } from '../models/rules';
import { EmptyRunResult, RunResult } from '../models/run-result';
import { CompileServerService } from '../services/compile-server.service';
import { RulesService } from '../services/rules.service';

@Component({
  selector: 'app-result',
  templateUrl: './result.component.html',
  styleUrls: ['./result.component.css']
})
export class ResultComponent implements OnInit {

  constructor(private compileServer: CompileServerService,
              private rulesService: RulesService) {
  }

  result: RunResult = EmptyRunResult;

  ngOnInit(): void {
    this.compileServer.lastExpressionResult.subscribe(rr => this.result = rr);
  }

  filteredStderr() {
    return this.result.stderr
      ? this.rulesService.filter(ErrorType.stderr, this.result.stderr)
      : ''
  }

  get previousExpressionResult() {
    return this.result.stdout || this.filteredStderr();
  }
}
