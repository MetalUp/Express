using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class OrganisationAuthorizer : ITypeAuthorizer<Organisation>
    {
        public bool IsVisible(Organisation org, string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                >= Role.Teacher => IsOwnOrganisation(org, context),
                Role.Student => IsOwnOrganisation(org, context) && IsProperty<Organisation>(memberName),
                _ => false
            };

        private bool IsOwnOrganisation(Organisation org, IContext context) =>
            Users.Me(context).OrganisationId == org.Id;
    }
}
