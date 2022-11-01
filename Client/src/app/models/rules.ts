export enum Applicability {
    both = 'both',
    expressions = 'expressions',
    functions = 'functions'
  }
  
  export enum ErrorType {
    cmpinfo = 'cmpinfo',
    stderr = 'stderr'
  }
  
  export interface ICodeRulesBlock {
    both: [string, string][],
    expressions: [string, string][],
    functions: [string, string][]
  }
  
  export interface IFilterRulesBlock {
    cmpinfo: string,
    stderr: string
  }
  
  export const MsgPrefix = "Messages.";

  export interface IRules {
    "Messages": IMessages,
    "ServerResponseMessageFilters": IFilterRulesBlock,
    "CodeMustMatch": ICodeRulesBlock,
    "CodeMustNotContain": ICodeRulesBlock
  }
  
  export interface IMessages {
    [key: string]: string
  }

  export const EmptyCodeRulesBlock : ICodeRulesBlock = {
    both: [],
    expressions: [],
    functions: []
  }