import { ICodeRulesBlock, IMessages } from "./rules";

export interface ITask {
    Language: string;
    Title: string;
    Description: string;
    Messages?: IMessages,
    CodeMustMatch?: ICodeRulesBlock,
    CodeMustNotContain?: ICodeRulesBlock,
    WrappingCode?: string,
    SkeletonCode?: string;
    PasteExpression?: boolean,
    PasteFunction?: boolean
}

export const EmptyTask = {
    Language: '',
    Title: '',
    Description: ''
}