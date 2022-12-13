using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class UserViewModelAuthorizer : ITypeAuthorizer<UserViewModel>
    {
        public bool IsVisible(UserViewModel uvm, string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                _ => uvm.UserId == Users.Me(context).Id
            };
    }
}
