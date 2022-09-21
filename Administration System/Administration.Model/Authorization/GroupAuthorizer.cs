using NakedFunctions.Security;

namespace Model.Authorization
{
    public class GroupAuthorizer : ITypeAuthorizer<Group>
    {
        public bool IsVisible(Group group, string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                >= Role.Teacher => Users.Me(context).OrganisationId == group.OrganisationId,
                _ => false
            };
    }
}
