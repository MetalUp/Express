import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Applicability, ErrorType, IRules, ICodeRulesBlock, MsgPrefix, EmptyCodeRulesBlock, ITaskRules } from '../models/rules';
import { TaskService } from './task.service';

export function rulesFactory(rules: RulesService) {
  return () => rules.load();
}

@Injectable({
  providedIn: 'root'
})
export class RulesService {

  constructor(private http: HttpClient, taskService: TaskService) {
    taskService.currentTask.subscribe(t => {
      this.taskRules.Messages = t.Messages || {};
      this.taskRules.CodeMustMatch = t.CodeMustMatch || EmptyCodeRulesBlock;
      this.taskRules.CodeMustNotContain = t.CodeMustNotContain || EmptyCodeRulesBlock;
    })
  }

  rules: IRules = { "Messages": {}, "ServerResponseMessageFilters": {}, CodeMustMatch: {}, CodeMustNotContain: {} };
  taskRules : ITaskRules  = {"Messages": {}, CodeMustMatch: EmptyCodeRulesBlock, CodeMustNotContain: EmptyCodeRulesBlock};

  private getRules(applicability: Applicability, rulesForLanguage: ICodeRulesBlock) {
    const applicableRules = rulesForLanguage[applicability] || [];
    const applicableBothRules = rulesForLanguage[Applicability.both] || [];
    return applicableRules.concat(applicableBothRules);
  }

  private getMustNotContainRules(language: string, applicability: Applicability) {
    const rulesForLanguage = this.rules.CodeMustNotContain[language] || EmptyCodeRulesBlock;
    const taskRules = this.taskRules.CodeMustNotContain || EmptyCodeRulesBlock;
    return this.getRules(applicability, rulesForLanguage).concat(this.getRules(applicability, taskRules));
  }

  private getMustMatchRules(language: string, applicability: Applicability) {
    const rulesForLanguage = this.rules.CodeMustMatch[language] || EmptyCodeRulesBlock;
    const taskRules = this.taskRules.CodeMustMatch || EmptyCodeRulesBlock;
    return this.getRules(applicability, rulesForLanguage).concat(this.getRules(applicability, taskRules));
  }

  private format(toformat: string, groups: RegExpMatchArray) {
    let i = 0;
    for (const replaceWith of groups) {
      const ph = `{${i++}}`;
      toformat = toformat.replace(ph, replaceWith);
    }
    return toformat;
  }

  private get taskMessages() {
    return this.taskRules.Messages;
  }

  private getMessage(origMessage: string) {
    return origMessage.startsWith(MsgPrefix)
      ? this.rules.Messages[origMessage.replace(MsgPrefix, '')] || this.taskMessages[origMessage.replace(MsgPrefix, '')] || origMessage
      : origMessage;
  }

  private handleError(e: unknown, regex: string) {
    if (e instanceof SyntaxError) {
      console.warn(e.message);
    }
    else {
      console.warn(`Uknown error with regex ${regex}`);
    }
    return '';
  }


  private mustNotContainRule(rs: RulesService, toValidate: string, rule: [re: string, msg: string]) {
    try {
      const re = new RegExp(rule[0]);
      const m = toValidate.match(re) || [];
      return m.length > 0 ? rs.format(rs.getMessage(rule[1]), m) : '';
    }
    catch (e) {
      return rs.handleError(e, rule[0]);
    }
  }

  private mustMatchRule(rs: RulesService, toParse: string, rule: [re: string, msg: string]) {
    try {
      const re = new RegExp(rule[0]);
      const m = toParse.match(re) || [];
      return m.length === 0 ? rs.getMessage(rule[1]) : '';
    }
    catch (e) {
      return rs.handleError(e, rule[0]);
    }
  }

  private handle(rules: [string, string][], toHandle: string, handler: (rs: RulesService, a: string, b: [string, string]) => string) {
    for (const rule of rules) {
      const msg = handler(this, toHandle, rule);
      if (msg) {
        return msg;
      }
    }
    return '';
  }

  public mustNotContain(language: string, applicability: Applicability, toCheck: string) {
    const rules = this.getMustNotContainRules(language, applicability);
    return this.handle(rules, toCheck, this.mustNotContainRule);
  }

  public mustMatch(language: string, applicability: Applicability, toCheck: string) {
    const rules = this.getMustMatchRules(language, applicability);
    return this.handle(rules, toCheck, this.mustMatchRule);
  }

  public checkRules(language: string, applicability: Applicability, toCheck: string) {
    return this.mustMatch(language, applicability, toCheck) ||
           this.mustNotContain(language, applicability, toCheck);
  }

  public filter(language: string, errorType: ErrorType, toFilter: string) {
    if (toFilter) {
      const rule = this.rules.ServerResponseMessageFilters[language][errorType];
      const re = new RegExp(rule);
      const m = re.exec(toFilter);
      return m ? m[0] : toFilter;
    }
    return toFilter;
  }

  load() {

    const options = {
      withCredentials: true,
    }

    this.http.get<IRules>('rules.json', options).subscribe(rules => this.rules = rules);
  }
}
