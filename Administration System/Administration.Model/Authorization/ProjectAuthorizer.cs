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
                    Role.Student => false,
                    _ => false
                };

        internal static bool AuthorAuthorization(Project proj, string memberName, IContext context) =>
           proj.IsAssignable() ? TeacherAuthorization(proj, memberName,context) : UserIsAuthor(proj, context);

        internal static bool TeacherAuthorization(Project proj, string memberName, IContext context) =>
           proj.IsAssignable() && 
            (IsTaskProperty(memberName) || 
                MatchesOneOf(memberName,
                    nameof(Project_Functions.AssignToMe),
                    nameof(Project_Functions.AssignToIndividual),
                    nameof(Project_Functions.AssignToGroup)));

        private static bool IsTaskProperty(string memberName) => IsProperty<Task>(memberName);

        private static bool UserIsAuthor(Project proj, IContext context) =>
            proj.AuthorId == Users.Me(context).Id;
    }
}
