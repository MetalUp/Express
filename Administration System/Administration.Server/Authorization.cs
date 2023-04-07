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
            config.AddTypeAuthorizer<UserViewModel, UserViewModelAuthorizer>();
            config.AddTypeAuthorizer<Organisation, OrganisationAuthorizer>();
            config.AddTypeAuthorizer<Group, GroupAuthorizer>();
            config.AddTypeAuthorizer<Project, ProjectAuthorizer>();
            config.AddTypeAuthorizer<Language, LanguageAuthorizer>();
            config.AddTypeAuthorizer<Task, TaskAuthorizer>();
            config.AddTypeAuthorizer<TaskUserView, TaskUserViewAuthorizer>();
            config.AddTypeAuthorizer<Hint, HintAuthorizer>();
            config.AddTypeAuthorizer<HintUserView, HintUserViewAuthorizer>();
            config.AddTypeAuthorizer<File, FileAuthorizer>();
            config.AddTypeAuthorizer<Assignment, AssignmentAuthorizer>();
            config.AddTypeAuthorizer<RunResult, RunResultAuthorizer>();
            config.AddTypeAuthorizer<CodeUserView, CodeUserViewAuthorizer>();
            config.AddTypeAuthorizer<FileViewModel, FileViewModelAuthorizer>();
            config.AddTypeAuthorizer<Activity, ActivityAuthorizer>();
            config.AddTypeAuthorizer<LanguageViewModel, LanguageViewModelAuthorizer>();
            return config;
        }
    }
}
