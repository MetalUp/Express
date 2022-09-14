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
    PreviousTask?: string,
    NextTaskDoesNotClearFunctions?: boolean
}

export class Task implements ITask {
    Language: string = "";
    Title: string = "";
    Description: string = "";
    Hints: string[] = [];
    Messages?: IMessages | undefined;
    CodeMustMatch?: ICodeRulesBlock | undefined;
    CodeMustNotContain?: ICodeRulesBlock | undefined;
    ReadyMadeFunctions?: string | undefined;
    SkeletonCode?: string | undefined;
    PasteExpression?: boolean | undefined;
    PasteFunction?: boolean | undefined;
    Tests?: string | undefined;
    NextTask?: string | undefined;
    PreviousTask?: string | undefined;
    NextTaskDoesNotClearFunctions?: boolean | undefined;
}


export const EmptyTask = {
    Language: '',
    Title: '',
    Description: '',
    Hints: []
}