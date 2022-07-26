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

interface IRules {
   "FilterRules" : IFilterRules,
   "ValidationRules": IValidationRules
}

interface IFilterRules {
  [key:string] : IFilterRulesBlock
}

interface IValidationRules {
  [key:string] : IValidationRulesBlock
}


@Injectable({
  providedIn: 'root'
})
export class RulesService {

  constructor(private http: HttpClient) { }

  private rules: IRules = {"FilterRules" : {}, ValidationRules: {}};

  getRules(language: string, applicability: Applicability) {

    const rulesForLanguage = this.rules.ValidationRules[language] as IValidationRulesBlock || [];

    const applicableRules = rulesForLanguage[applicability] || [];
    const applicableBothRules = rulesForLanguage[Applicability.both] || [];

    return applicableRules.concat(applicableBothRules);
  }

  private validateRule(toValidate: string, rule : [re: string, msg: string]) {
    const re = new RegExp(rule[0]);
    const m =  toValidate.match(re) || [];
    return m.length > 0 ? rule[1].replace("{match}", m[0]) : '';
  }

  public validate(language: string, applicability: Applicability, toValidate: string){
    const rules = this.getRules(language, applicability);

    for(const rule of rules) {
      const msg = this.validateRule(toValidate, rule);
      if (msg){
        return msg;
      }  
    }
    return '';
  } 

  public filter(language: string, errorType: ErrorType, toFilter: string){
    if (toFilter) {
      const rule = this.rules.FilterRules[language][errorType];
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
