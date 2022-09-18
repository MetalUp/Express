using Model.Functions;
using Model.Types;
using NakedFramework.Metamodel.Authorization;
using NakedFunctions;
using NakedFunctions.Reflector.Authorization;
using NakedFunctions.Security;
using Model.Authorization;

namespace Server
{
    public static class AuthorizationHelpers
    {
        public static IAuthorizationConfiguration AdminAuthConfig()
        {
            var config = new AuthorizationConfiguration<DefaultTypeAuthorizer, MainMenuAuthorizer>();
            config.AddTypeAuthorizer<User, UserAuthorizer>();
            config.AddTypeAuthorizer<Invitation, InvitationAuthorizer>();
            config.AddTypeAuthorizer<Task, TaskAuthorizer>();
            return config;
        }


    }

}
