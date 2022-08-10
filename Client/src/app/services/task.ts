import { ICodeRulesBlock, IMessages } from "./rules";

export interface ITask {
    Language: string;
    Title: string;
    Description: string;
    Hints: string[];
    Messages?: IMessages,
    CodeMustMatch?: ICodeRulesBlock,
    CodeMustNotContain?: ICodeRulesBlock,
    ReadyMadeFunctions?: string,
    SkeletonCode?: string;
    PasteExpression?: boolean,
    PasteFunction?: boolean,
    Tests?: string,
    NextTask?: string,
    PreviousTask?: string
}

export const EmptyTask = {
    Language: '',
    Title: '',
    Description: '',
    Hints: []
}