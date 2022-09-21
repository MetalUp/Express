using NakedFunctions.Security;

namespace Model.Authorization
{
    public class OrganisationAuthorizer : ITypeAuthorizer<Organisation>
    {
        public bool IsVisible(Organisation org, string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                >= Role.Teacher => Users.Me(context).OrganisationId == org.Id,
                _ => false
            };
    }
}
