export interface ITaskUserView {
    Id: number;
    NextTaskEnabled: boolean;
    Title: string;
    Language: string;
    Description: string;
    RegExRules: string;
    PasteExpression: boolean;
    PasteCode: boolean;
    PreviousTaskId?: number;
    NextTaskId?: number;
    Code: string;
    HasTests: boolean;
}

export class TaskUserView implements ITaskUserView {
  
    constructor(public Id: number) {}
    NextTaskEnabled = false;
    Title = "";
    Language = "";
    Description = "";
    RegExRules = "";
    PasteExpression= false;
    PasteCode = false;
    NextTaskClearsFunctions = false;
    Code = "";
    HasTests = false;
}

export const EmptyTaskUserView = new TaskUserView(0);