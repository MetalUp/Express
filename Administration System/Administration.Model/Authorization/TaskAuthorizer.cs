using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class TaskAuthorizer : ITypeAuthorizer<Task>
    {

        public bool IsVisible(Task task, string memberName, IContext context) =>
                Users.UserRole(context) switch
                {
                    Role.Root => true,
                    Role.Author => IsTaskProperty(memberName) || UserIsAuthor(task, context),//TODO: actions only for the author
                    Role.Teacher => IsTaskProperty(memberName),
                    Role.Student => StudentAuthorization(task, memberName, context),
                    _ => false
                };


        internal static bool StudentAuthorization(Task task, string memberName, IContext context) =>
            TaskIsAssignedToUser(task, context) && IsTaskProperty(memberName);

        internal static bool TaskIsAssignedToUser(Task task, IContext context) =>
            task.Project.IsAssignedToMe(context);

        private static bool IsTaskProperty(string memberName) => IsProperty<Task>(memberName);

        private bool UserIsAuthor(Task task, IContext context) =>
            task.Project.AuthorId == Users.Me(context).Id;


    }
}
