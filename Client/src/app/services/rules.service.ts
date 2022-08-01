import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export function rulesFactory(rules: RulesService) {
  return () => rules.load();
}

export enum Applicability {
  both = 'both',
  expressions = 'expressions',
  functions = 'functions'
}

export enum ErrorType {
  cmpinfo = 'cmpinfo',
  stderr = 'stderr'
}

interface IValidationRulesBlock {
  both: [string, string][],
  expressions: [string, string][],
  functions: [string, string][]
}

interface IFilterRulesBlock {
  cmpinfo: string,
  stderr: string
}

const MsgPrefix = "Messages.";

export interface IRules {
  "Messages": IMessages,
  "ServerResponseMessageFilters": IFilterRules,
  "CodeMustMatch": IValidationRules,
  "CodeMustNotContain": IValidationRules
}

interface IFilterRules {
  [key: string]: IFilterRulesBlock
}

interface IValidationRules {
  [key: string]: IValidationRulesBlock
}

interface IMessages {
  [key: string]: string
}


@Injectable({
  providedIn: 'root'
})
export class RulesService {

  constructor(private http: HttpClient) { }

  rules: IRules = { "Messages": {}, "ServerResponseMessageFilters": {}, CodeMustMatch: {}, CodeMustNotContain: {} };

  private getRules(applicability: Applicability, rulesForLanguage: IValidationRulesBlock) {
    const applicableRules = rulesForLanguage[applicability] || [];
    const applicableBothRules = rulesForLanguage[Applicability.both] || [];
    return applicableRules.concat(applicableBothRules);
  }

  private getValidationRules(language: string, applicability: Applicability) {
    const rulesForLanguage = this.rules.CodeMustNotContain[language] as IValidationRulesBlock || [];
    return this.getRules(applicability, rulesForLanguage);
  }

  private getParsingRules(language: string, applicability: Applicability) {
    const rulesForLanguage = this.rules.CodeMustMatch[language] as IValidationRulesBlock || [];
    return this.getRules(applicability, rulesForLanguage);
  }

  private format(toformat: string, groups: RegExpMatchArray) {
    let i = 0;
    for (const replaceWith of groups) {
      const ph = `{${i++}}`;
      toformat = toformat.replace(ph, replaceWith);
    }
    return toformat;
  }

  private getMessage(origMessage: string) {
    return origMessage.startsWith(MsgPrefix) ? this.rules.Messages[origMessage.replace(MsgPrefix, '')] || origMessage : origMessage;
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


  private validateRule(rs: RulesService, toValidate: string, rule: [re: string, msg: string]) {
    try {
      const re = new RegExp(rule[0]);
      const m = toValidate.match(re) || [];
      return m.length > 0 ? rs.format(rs.getMessage(rule[1]), m) : '';
    }
    catch (e) {
      return rs.handleError(e, rule[0]);
    }
  }

  private parseRule(rs: RulesService, toParse: string, rule: [re: string, msg: string]) {
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

  public validate(language: string, applicability: Applicability, toValidate: string) {
    const rules = this.getValidationRules(language, applicability);
    return this.handle(rules, toValidate, this.validateRule);
  }

  public parse(language: string, applicability: Applicability, toParse: string) {
    const rules = this.getParsingRules(language, applicability);
    return this.handle(rules, toParse, this.parseRule);
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
