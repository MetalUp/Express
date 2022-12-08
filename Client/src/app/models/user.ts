export interface IUser {
   Name : string;
   Role: string,
   Status: string;
}

export class User implements IUser {
    Name = "";
    Role = "";
    Status = ";"
}

export const EmptyUser = new User();