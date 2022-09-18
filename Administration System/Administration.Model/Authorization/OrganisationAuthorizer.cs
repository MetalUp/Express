using NakedFunctions.Security;

namespace Model.Authorization
{
    public class OrganisationAuthorizer : ITypeAuthorizer<Organisation>
    {
        public bool IsVisible(Organisation org, string memberName, IContext context) =>
            UserRepository.UserRole(context) switch
            {
                >= Role.Teacher => UserRepository.Me(context).OrganisationId == org.Id,
                _ => false
            };
    }
}
