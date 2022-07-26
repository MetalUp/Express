import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export function rulesFactory(rules: RulesService) {
  return () => rules.load();
}

enum Applicability {
  both = 'both',
  expressions = 'expressions',
  functions = 'functions'
}

function isApplicability(s: string) : s is Applicability {
  return Object.values(Applicability).includes(s as Applicability);
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

  public getRules(language: string, applicability: Applicability) {

    const rulesForLanguage = this.rules[language] as IRulesBlock || [];

    const applicableRules = rulesForLanguage[applicability] || [];
    const applicableBothRules = rulesForLanguage[Applicability.both] || [];

    return applicableRules.concat(applicableBothRules);
  }

  load() {

    const options = {
      withCredentials: true,
    }

    this.http.get<IRules>('rules.json', options).subscribe(rules => this.rules = rules);
  }
}
