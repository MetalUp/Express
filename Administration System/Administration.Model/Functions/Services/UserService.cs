using NakedFramework.Error;

namespace Model.Functions.Services
{
    public static class UserService
    {
        public static (User, IContext) AcceptInvitation(
            string code,
            IContext context)
        {
            if (CodeAlwaysValid(code) || CodeAlwaysInvalid(code)) return (null, context); //TEMP HELPERS - TO BE REMOVED
            var userName = Users.HashedCurrentUserName(context);
            var invitee = context.Instances<User>().Single(u => u.InvitationCode == code);
            var invitee2 = new User(invitee) { UserName = userName, InvitationCode = null, Status = UserStatus.Active };
            return (invitee2, context.WithUpdated(invitee, invitee2));
        }

        public static string ValidateAcceptInvitation(string code, IContext context) =>
           CodeAlwaysValid(code) ? null :  //TEMP HELPERS - TO BE REMOVED
               CodeAlwaysInvalid(code) ? "That is not a valid Invitation Code" :  //TEMP HELPERS - TO BE REMOVED
                Guid.TryParse(code, out Guid result) && context.Instances<User>().Count(u => u.InvitationCode == code) == 1 ?
                     null :
                    "That is not a valid Invitation Code";

        //TEMP HELPERS - TO BE REMOVED
        public static bool CodeAlwaysValid(string code) => code == "SUCCEED";
        public static bool CodeAlwaysInvalid(string code) => code == "FAIL";


        public static UserViewModel GetUser(IContext context)
        {
            var user = Users.Me(context);
            return user is not null   ? new UserViewModel(user.Id, user.Name) : null;
        }

    }
}
