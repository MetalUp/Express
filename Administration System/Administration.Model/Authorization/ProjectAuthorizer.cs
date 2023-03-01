using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class ProjectAuthorizer : ITypeAuthorizer<Project>
    {
        public bool IsVisible(Project proj, string memberName, IContext context) =>
                Users.UserRole(context) switch
                {
                    Role.Root => true,
                    Role.Author => AuthorAuthorization(proj, memberName, context),
                    Role.Teacher => TeacherAuthorization(proj, memberName, context),
                    Role.Student => StudentAuthorization(proj, memberName, context),
                    _ => false
                };

        internal static bool AuthorAuthorization(Project proj, string memberName, IContext context) =>
           IsProperty(memberName) ? true :
             MatchesOneOf(memberName,
                    nameof(Project_Functions.AssignToMe),
                    nameof(Project_Functions.AssignToIndividual),
                    nameof(Project_Functions.AssignToGroup)) ? true :
                        UserIsAuthor(proj, context);

        internal static bool TeacherAuthorization(Project proj, string memberName, IContext context) =>
           proj.IsAssignable() && 
            (IsProperty(memberName) || 
                MatchesOneOf(memberName,
                    nameof(Project_Functions.AssignToMe),
                    nameof(Project_Functions.AssignToIndividual),
                    nameof(Project_Functions.AssignToGroup)));

        internal static bool StudentAuthorization(Project proj, string memberName, IContext context) =>
           proj.IsAssignable() &&
            IsProperty<Project>(memberName) && !memberName.Contains("File");

        private static bool IsProperty(string memberName) => IsProperty<Project>(memberName);

        private static bool UserIsAuthor(Project proj, IContext context) =>
            proj.AuthorId == Users.Me(context).Id;
    }
}
