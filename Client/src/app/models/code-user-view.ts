export interface ICodeUserView {
    TaskId: number
    Code: string
    Version: number
    HasPreviousVersion: boolean;
}

export class CodeUserView implements ICodeUserView {
    TaskId = 0;
    Code = "";
    Version = -1;
    HasPreviousVersion = false;
}

export const EmptyCodeUserView = new CodeUserView();