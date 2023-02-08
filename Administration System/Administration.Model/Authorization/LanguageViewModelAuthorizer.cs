using NakedFunctions.Security;

namespace Model.Authorization; 

public class LanguageViewModelAuthorizer : ITypeAuthorizer<LanguageViewModel> {
    public bool IsVisible(LanguageViewModel lang, string memberName, IContext context) =>
        Users.UserRole(context) switch {
            Role.Root => true,
            _ => false
        };
}