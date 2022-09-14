using Model.Functions;
using Model.Types;
using NakedFramework.Metamodel.Authorization;
using NakedFunctions;
using NakedFunctions.Reflector.Authorization;
using NakedFunctions.Security;

namespace Server
{
    public static class AuthorizationHelpers
    {
        public static IAuthorizationConfiguration AdminAuthConfig()
        {
            var config = new AuthorizationConfiguration<AdminDefaultAuthorizer, AdminMainMenuAuthorizer>();
            //config.AddNamespaceAuthorizer<MyAppAuthorizer>("MyApp");
            //config.AddNamespaceAuthorizer<MyCluster1Authorizer>("MyApp.MyCluster1");
            //config.AddTypeAuthorizer<Bar, MyBarAuthorizer>();
            return config;
        }

        public static bool UserHasRoleAtLeast(Role role, IContext context)
        {
            var user = Users.Me(context);
            var usersRole = user == null ? Role.Guest : user.Role;
            //TODO: Can we safely statically cache all the above?
            return usersRole >= role;
        }

        public static bool UserHasSpecificRole(Role role, IContext context)
        {
            var user = Users.Me(context);
            var usersRole = user == null ? Role.Guest : user.Role;
            //TODO: Can we safely statically cache all the above?
            return usersRole == role;
        }
    }

    public class AdminDefaultAuthorizer : ITypeAuthorizer<object>
    {
        public bool IsVisible(object target, string memberName, IContext context)
            => AuthorizationHelpers.UserHasRoleAtLeast(Role.Student, context);
    }

    public class AdminMainMenuAuthorizer : IMainMenuAuthorizer
    {
        public bool IsVisible(string target, string memberName, IContext context)
        => AuthorizationHelpers.UserHasRoleAtLeast(Role.Student, context);
    }
}
