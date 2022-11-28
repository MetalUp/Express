export interface IHintUserView {
    Title: string;
    Contents: string;
    PreviousHintNo: number;//0 indicates there is no previous hint
    NextHintNo: number;//0 indicates there is no next hint
    CostOfNextHint: number; //If zero means that the user can just navigate to it (because they have seen it before)
    NextHintAlreadyUsed: boolean;
}

export class HintUserView implements IHintUserView {
    Title = '';
    Contents = '';
    PreviousHintNo = 0;
    NextHintNo = 0;
    CostOfNextHint = -1;
    NextHintAlreadyUsed = false;
}

export const EmptyHintUserView = new HintUserView();