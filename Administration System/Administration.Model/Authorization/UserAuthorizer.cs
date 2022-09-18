using NakedFunctions.Security;

namespace Model.Authorization
{
    public class UserAuthorizer : ITypeAuthorizer<User>
    {
        public bool IsVisible(User user, string memberName, IContext context) =>
            UserRepository.UserRole(context) switch
            {
                >= Role.Teacher => UserRepository.Me(context).OrganisationId == user.OrganisationId,
                _ => false
            };
    }
}
