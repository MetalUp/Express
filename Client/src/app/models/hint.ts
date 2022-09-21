export interface IHint {
    Number: number;
    Title: string;
    CostInMarks: number;
    HtmlFile: [string, string];
    Task: string;
}

export class Hint implements IHint
{
    Number = 0;
    Title = ''
    CostInMarks = 0;
    HtmlFile = ["", ""] as [string, string];
    Task = '';
}

export const EmptyHint = new Hint();