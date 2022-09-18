using NakedFunctions.Security;

namespace Model.Authorization
{
    public class GroupAuthorizer : ITypeAuthorizer<Group>
    {
        public bool IsVisible(Group group, string memberName, IContext context) =>
            UserRepository.UserRole(context) switch
            {
                >= Role.Teacher => UserRepository.Me(context).OrganisationId == group.OrganisationId,
                _ => false
            };
    }
}
