using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class UserAuthorizer : ITypeAuthorizer<User>
    {
        public bool IsVisible(User user, string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                >= Role.Teacher => Users.Me(context).OrganisationId == user.OrganisationId && 
                    (IsProperty<User>(memberName) ||
                    MatchesOneOf(memberName,  
                        nameof(User_Functions.AddToGroup),
                        nameof(User_Functions.EditName),
                        nameof(User_Functions.EditEmailAddress),
                        nameof(User_Functions.RecentAssignments),
                        nameof(User_Functions.InviteToChangeLoginCredentials),
                        nameof(User_Functions.SetToInactive),
                        nameof(User_Functions.RemoveIdentityInfo),
                        nameof(User_Functions.RecentActivity)
                        )),
                Role.Student => user.Id == Users.Me(context).Id && IsProperty<User>(memberName),
                _ => false
            };
    }
}
