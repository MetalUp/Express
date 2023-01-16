export interface ITaskUserView {
    Id: number;
    Completed: boolean;
    NextTaskIsStarted: boolean;
    Title: string;
    Language: string;
    Description: string;
    RegExRules: string;
    PasteExpression: boolean;
    PasteCode: boolean;
    PreviousTaskId?: number;
    NextTaskId?: number;
    Code?: string;
    HasTests: boolean;
    AssignmentId: number;
    ClientRunTestCode: string;
    TestRunLocally: boolean;
}

export class TaskUserView implements ITaskUserView {
  
    constructor(public Id: number) {}
    PreviousTaskId?: number | undefined;
    NextTaskId?: number | undefined;
    ClientRunTestCode = "";
    TestRunLocally = false;
    Completed = false;
    NextTaskIsStarted = false;
    Title = "";
    Language = "";
    Description = "";
    RegExRules = "";
    PasteExpression= false;
    PasteCode = false;
    NextTaskClearsFunctions = false;
    Code = "";
    HasTests = false;
    AssignmentId = 0;
}

export const EmptyTaskUserView = new TaskUserView(0);