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

interface IRulesBlock {
    both: [string, string][],
    expressions: [string, string][],
    functions: [string, string][]
}

interface IRules {
   [key:string] : IRulesBlock
}


@Injectable({
  providedIn: 'root'
})
export class RulesService {

  constructor(private http: HttpClient) { }

  private rules: IRules = {};

  getRules(language: string, applicability: Applicability) {

    const rulesForLanguage = this.rules[language] as IRulesBlock || [];

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

  load() {

    const options = {
      withCredentials: true,
    }

    this.http.get<IRules>('rules.json', options).subscribe(rules => this.rules = rules);
  }
}
