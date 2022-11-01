import { Injectable } from '@angular/core';
import { Applicability, ErrorType, IRules, ICodeRulesBlock, MsgPrefix, EmptyCodeRulesBlock } from '../models/rules';
import { TaskService } from './task.service';

@Injectable({
  providedIn: 'root'
})
export class RulesService {

  constructor(taskService: TaskService) {
    taskService.currentTask.subscribe(t => {
      this.taskRules = t.RegExRules ? JSON.parse(t.RegExRules) : null;
    })
  }

  taskRules? : IRules;

  private getRules(applicability: Applicability, rules: ICodeRulesBlock) {
    const applicableRules = rules[applicability] || [];
    const applicableBothRules = rules[Applicability.both] || [];
    return applicableRules.concat(applicableBothRules);
  }

  private getMustNotContainRules(applicability: Applicability) {
    const rules = this.taskRules?.CodeMustNotContain || EmptyCodeRulesBlock;
    return this.getRules(applicability, rules);
  }

  private getMustMatchRules(applicability: Applicability) {
    const rules = this.taskRules?.CodeMustMatch || EmptyCodeRulesBlock;
    return this.getRules(applicability, rules);
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
    return this.taskRules?.Messages;
  }

  private getMessage(origMessage: string) {
    return origMessage.startsWith(MsgPrefix)
      ? this.taskMessages?.[origMessage.replace(MsgPrefix, '')] || origMessage
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

  public mustNotContain(applicability: Applicability, toCheck: string) {
    const rules = this.getMustNotContainRules(applicability);
    return this.handle(rules, toCheck, this.mustNotContainRule);
  }

  public mustMatch(applicability: Applicability, toCheck: string) {
    const rules = this.getMustMatchRules(applicability);
    return this.handle(rules, toCheck, this.mustMatchRule);
  }

  public checkRules(applicability: Applicability, toCheck: string) {
    return this.mustMatch(applicability, toCheck) ||
           this.mustNotContain(applicability, toCheck);
  }

  public filter(errorType: ErrorType, toFilter: string) {
    if (toFilter) {
      const rule = this.taskRules?.ServerResponseMessageFilters[errorType];
      if(rule) {
        const re = new RegExp(rule);
        const m = re.exec(toFilter);
        return m ? m[0] : toFilter;
      }
    }
    return toFilter;
  }
}
