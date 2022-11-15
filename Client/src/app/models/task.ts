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