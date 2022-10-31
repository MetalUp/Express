using Model.Types;
using NakedFramework.Metamodel.Authorization;
using NakedFunctions.Reflector.Authorization;
using Model.Authorization;
using Model.Functions.Services;

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
            config.AddTypeAuthorizer<Project, ProjectAuthorizer>();
            config.AddTypeAuthorizer<Language, LanguageAuthorizer>();
            config.AddTypeAuthorizer<Task, TaskAuthorizer>();
            config.AddTypeAuthorizer<Hint, HintAuthorizer>();
            config.AddTypeAuthorizer<File, FileAuthorizer>();
            config.AddTypeAuthorizer<Assignment, AssignmentAuthorizer>();
            return config;
        }


    }

}
