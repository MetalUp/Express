import { Component, OnInit } from '@angular/core';
import { EmptyRunResult, RunResult } from '../models/run-result';
import { CompileServerService } from '../services/compile-server.service';

@Component({
  selector: 'app-result',
  templateUrl: './result.component.html',
  styleUrls: ['./result.component.css']
})
export class ResultComponent implements OnInit {

  constructor(private compileServer: CompileServerService) {
  }

  lastRunResult: RunResult = EmptyRunResult;

  ngOnInit(): void {
    this.compileServer.lastExpressionResult.subscribe(rr => this.lastRunResult = rr);
  }

  get previousExpressionResult() {
    return this.lastRunResult.stdout;
  }
}
