import { IHint } from "./hint";
import { ICodeRulesBlock, IMessages, IRules } from "./rules";

export interface ITask {
    Id: number;
    Language: string;
    Title: string;
    Description: [string, string];
    Hints: IHint[];
    HasTests: boolean;
    RegExRules?: string;
    PasteExpression?: boolean;
    PasteCode?: boolean;
    NextTask?: string;
    PreviousTask?: string;
    NextTaskClearsFunctions?: boolean;
}

export class Task implements ITask {
  
    constructor(public Id: number) {}
    Title: string = "";
    Description: [string, string] = ["", ""];
    Language: string = "";
    Hints: IHint[] = [];
    HasTests: boolean = false;
    RegExRules?: string;
    PasteExpression?: boolean | undefined;
    PasteCode?: boolean | undefined;
    Tests?: [string, string];
    NextTask?: string | undefined;
    PreviousTask?: string | undefined;
    NextTaskClearsFunctions?: boolean | undefined;
}

export const EmptyTask = new Task(0);