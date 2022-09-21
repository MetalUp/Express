using Model.Functions;
using NakedFunctions.Security;

namespace Model.Authorization
{

    public class DefaultTypeAuthorizer : ITypeAuthorizer<object>
    {
        public bool IsVisible(object target, string memberName, IContext context)
            => Users.UserHasRoleAtLeast(Role.Root, context);
    }
}
