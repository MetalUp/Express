export interface IUserView {
   DisplayName : string;
}

export class UserView implements IUserView {
    DisplayName = "";
}

export const EmptyUserView = new UserView();