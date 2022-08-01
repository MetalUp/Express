import { ICodeRulesBlock } from "./rules";

export interface ITask {
    language: string;
    title: string;
    description: string;
    CodeMustMatch?: ICodeRulesBlock,
    CodeMustNotContain?: ICodeRulesBlock
}

export const EmptyTask = {
    language: '',
    title: '',
    description: ''
}