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
                    _ => GuestAuthorization(task, memberName, context)
                };


        internal static bool StudentAuthorization(Task task, string memberName, IContext context) =>
            TaskIsDefaultOrAssignedToUser(task, context) && IsTaskProperty(memberName);

        internal static bool TaskIsDefaultOrAssignedToUser(Task task, IContext context) =>
            task.IsDefault() || task.Project.IsAssignedToCurrentUser(context);

        internal static bool GuestAuthorization(Task task, string memberName, IContext context) =>
            task.IsDefault() && IsTaskProperty(memberName);

        private static bool IsTaskProperty(string memberName) => IsProperty<Task>(memberName);

        private bool UserIsAuthor(Task task, IContext context) =>
            task.Project.AuthorId == Users.Me(context).Id;


    }
}
