﻿using NakedFramework.Error;

namespace Model.Services
{
    public static class UserService
    {
        public static (User, IContext) AcceptInvitation(
            string code,
            IContext context)
        {
            var existingUser = Users.Me(context);
            if (existingUser != null) return (existingUser, context.WithWarnUser($"User is already registered, so cannot accept new invitation."));
            var invitee = context.Instances<User>().Single(u => u.InvitationCode == code);
            var hashedUserName = Users.HashedCurrentUserName(context);
            var invitee2 = new User(invitee) { UserName = hashedUserName, InvitationCode = null, Status = UserStatus.Active };
            return (invitee2, context.WithUpdated(invitee, invitee2));
        }

        public static string ValidateAcceptInvitation(string code, IContext context) =>
                Guid.TryParse(code, out Guid result) && context.Instances<User>().Count(u => u.InvitationCode == code) == 1 ?
                     null :
                    "That is not a valid Invitation Code";

        public static UserViewModel GetUser(IContext context)
        {
            var user = Users.Me(context);
            if (user == null)
            {
                return null;
            }
            else
            {
                int taskId = Tasks.MyLastActiveTaskId(context);
                return new UserViewModel(user.Id, user.Name, taskId);
            }
        }
    }
}
