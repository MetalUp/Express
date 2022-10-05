using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class TaskAuthorizer : ITypeAuthorizer<Task>
    {
        //TODO: Student must not be able to access the content files of a task directly;
        // so only author has access to methods to retrieve (or save) a content file associated with a task

        public bool IsVisible(Task task, string memberName, IContext context) =>
                Users.UserRole(context) switch
                {
                    >= Role.Teacher => true,
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
    }
}
