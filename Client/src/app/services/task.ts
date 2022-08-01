import { ICodeRulesBlock } from "./rules";

export interface ITask {
    language: string;
    title: string;
    description: string;
    CodeMustMatch?: ICodeRulesBlock,
    CodeMustNotContain?: ICodeRulesBlock,
    wrappingCode?: string,
    skeletonCode?: string;
    pasteExpression?: boolean,
    pasteFunction?: boolean
}

export const EmptyTask = {
    language: '',
    title: '',
    description: ''
}