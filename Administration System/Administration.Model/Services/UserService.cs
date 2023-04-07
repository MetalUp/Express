using NakedFramework.Error;

namespace Model.Services
{
    public static class UserService
    {
        public static (User, IContext) AcceptInvitation(
            string code,
            IContext context)
        {
            var userName = Users.HashedCurrentUserName(context);
            var invitee = context.Instances<User>().Single(u => u.InvitationCode == code);
            var invitee2 = new User(invitee) { UserName = userName, InvitationCode = null, Status = UserStatus.Active };
            return (invitee2, context.WithUpdated(invitee, invitee2));
        }

        public static string ValidateAcceptInvitation(string code, IContext context) =>
                Guid.TryParse(code, out Guid result) && context.Instances<User>().Count(u => u.InvitationCode == code) == 1 ?
                     null :
                    "That is not a valid Invitation Code";

        public static UserViewModel GetUser(IContext context)
        {
            var user = Users.Me(context);
            int taskId = Tasks.MyLastActiveTaskId(context);
            return user is not null   ? new UserViewModel(user.Id, user.Name, taskId) : null;
        }
    }
}
