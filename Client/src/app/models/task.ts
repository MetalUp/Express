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


export interface ITaskUserView {
    Id: number;
    Title: string;
    Language: string;
    Description: string;
    RegExRules: string;
    PasteExpression: boolean;
    PasteCode: boolean;
    PreviousTaskId?: number;
    NextTaskId?: number;
    NextTaskClearsFunctions: boolean
    NextTaskEnabled: boolean;
    CodeLastSubmitted: string;
    PreviousHintNo?: number;
    CurrentHintNo: number;
    CurrentHintTitle: string;
    CurrentHintContent: string;
    NextHintNo?: number;
}

export class TaskUserView implements ITaskUserView {
  
    constructor(public Id: number) {}
    Title = "";
    Language = "";
    Description = "";
    RegExRules = "";
    PasteExpression= false;
    PasteCode = false;
    NextTaskClearsFunctions = false;
    NextTaskEnabled = false;
    CodeLastSubmitted = "";
    CurrentHintNo = 0;
    CurrentHintTitle = "";
    CurrentHintContent = "";  
}

export const EmptyTaskUserView = new TaskUserView(0);