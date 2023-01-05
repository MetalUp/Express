using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class HintAuthorizer : ITypeAuthorizer<Hint>
    {

        public bool IsVisible(Hint hint, string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                Role.Author => AuthorAuthorization(hint, memberName, context),
                _ => false
            };

        private bool AuthorAuthorization(Hint hint, string memberName, IContext context) =>
            IsProperty<Hint>(memberName);

    }
}
