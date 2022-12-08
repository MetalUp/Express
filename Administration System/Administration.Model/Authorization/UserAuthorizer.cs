using NakedFunctions.Security;

namespace Model.Authorization
{
    public class UserAuthorizer : ITypeAuthorizer<User>
    {
        public bool IsVisible(User user, string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                >= Role.Teacher => Users.Me(context).OrganisationId == user.OrganisationId,
                Role.Student => user.Id == Users.Me(context).Id,
                _ => false
            };
    }
}
