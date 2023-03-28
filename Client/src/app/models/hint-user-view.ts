export interface IHintUserView {
    HintNo: number;
    Title: string;
    Contents: string;
    PreviousHintNo: number;//0 indicates there is no previous hint
    NextHintNo: number;//0 indicates there is no next hint
    NextHintAlreadyUsed: boolean;
}

export class HintUserView implements IHintUserView {
    HintNo = 0;
    Title = '';
    Contents = '';
    PreviousHintNo = 0;
    NextHintNo = 0;
    NextHintAlreadyUsed = false;
}

export const EmptyHintUserView = new HintUserView();