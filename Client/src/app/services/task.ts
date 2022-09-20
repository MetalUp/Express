import { ICodeRulesBlock, IMessages } from "./rules";

export interface ITask {
    Language: string;
    Title: string;
    Description: [string, string];
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
    private language = "";
    
    get Language() {
        return this.language;
    }

    set Language(l : string) {
        this.language = l.replace(' ', '').toLowerCase(); // to fix 'C Sharp'
    }
    
    Title: string = "";
    Description: [string, string] = ["", ""];
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
    Description: ['', ''] as [string, string],
    Hints: []
}