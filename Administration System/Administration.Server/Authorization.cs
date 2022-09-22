using Model.Types;
using NakedFramework.Metamodel.Authorization;
using NakedFunctions.Reflector.Authorization;
using Model.Authorization;

namespace Server
{
    public static class AuthorizationHelpers
    {
        public static IAuthorizationConfiguration AdminAuthConfig()
        {
            var config = new AuthorizationConfiguration<DefaultTypeAuthorizer, MainMenuAuthorizer>();
            config.AddTypeAuthorizer<User, UserAuthorizer>();
            config.AddTypeAuthorizer<Organisation, OrganisationAuthorizer>();
            config.AddTypeAuthorizer<Group, GroupAuthorizer>();
            config.AddTypeAuthorizer<Task, TaskAuthorizer>();
            config.AddTypeAuthorizer<Hint, HintAuthorizer>();
            config.AddTypeAuthorizer<Assignment, AssignmentAuthorizer>();
            return config;
        }


    }

}
