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
           proj.IsAssignable() || proj.AuthorId == Users.Me(context).Id;

        internal static bool TeacherAuthorization(Project proj, string memberName, IContext context) =>
           proj.IsAssignable() && 
            (IsTaskProperty(memberName) || 
                MatchesOneOf(memberName,
                    nameof(Project_Functions.AssignToGroup),
                    nameof(Project_Functions.AssignToIndividual)));

        private static bool IsTaskProperty(string memberName) => IsProperty<Task>(memberName);
    }
}
