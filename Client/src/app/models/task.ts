import { IHint } from "./hint";
import { ICodeRulesBlock, IMessages } from "./rules";

export interface ITask {
    Language: string;
    Title: string;
    Description: [string, string];
    Hints: IHint[];
    Messages?: IMessages;
    CodeMustMatch?: ICodeRulesBlock;
    CodeMustNotContain?: ICodeRulesBlock;
    ReadyMadeFunctions?:  [string, string];
    SkeletonCode?: string;
    PasteExpression?: boolean;
    PasteFunction?: boolean;
    Tests?: [string, string];
    NextTask?: string;
    PreviousTask?: string;
    NextTaskDoesNotClearFunctions?: boolean;
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
    Hints: IHint[] = [];
    Messages?: IMessages | undefined;
    CodeMustMatch?: ICodeRulesBlock;
    CodeMustNotContain?: ICodeRulesBlock;
    ReadyMadeFunctions?: [string, string];
    SkeletonCode?: string | undefined;
    PasteExpression?: boolean | undefined;
    PasteFunction?: boolean | undefined;
    Tests?: [string, string];
    NextTask?: string | undefined;
    PreviousTask?: string | undefined;
    NextTaskDoesNotClearFunctions?: boolean | undefined;
}

export const EmptyTask = new Task();