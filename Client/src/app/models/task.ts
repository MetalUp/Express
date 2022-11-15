export interface ITaskUserView {
    Id: number;
    IsCompleted: boolean;
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
}

export class TaskUserView implements ITaskUserView {
  
    constructor(public Id: number) {}
    IsCompleted = false;
    Title = "";
    Language = "";
    Description = "";
    RegExRules = "";
    PasteExpression= false;
    PasteCode = false;
    NextTaskClearsFunctions = false;
    NextTaskEnabled = false;
    CodeLastSubmitted = "";
}

export const EmptyTaskUserView = new TaskUserView(0);