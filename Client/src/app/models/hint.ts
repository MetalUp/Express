export interface IHintUserView {
    Title: string;
    Contents: string;
    PreviousHintNo?: number;//Null indicates there is no previous hint
    NextHintNo?: number;//Null indicates there is no next hint
    CostOfNextHint: number; //If zero means that the user can just navigate to it (because they have seen it before)
}

export class HintUserView implements IHintUserView {

    Title = '';
    Contents = '';
    CostOfNextHint = -1;
}

export const EmptyHintUserView = new HintUserView();