export interface IUserView {
   DisplayName : string;
   Registered? : boolean;
}

export class UserView implements IUserView {
    DisplayName = "";
}

export const EmptyUserView = new UserView();
export const UnregisteredUserView = { Registered: false } as IUserView;
export const RegisteredUserView = { Registered: true } as IUserView;