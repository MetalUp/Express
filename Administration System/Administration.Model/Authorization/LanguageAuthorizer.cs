using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class LanguageAuthorizer : ITypeAuthorizer<Language>
    {
        public bool IsVisible(Language lang, string memberName, IContext context) =>
         Users.UserRole(context) switch
            {
                Role.Root => true,
                _ => false,
            };
    }
}
