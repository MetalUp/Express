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
                    Role.Root => true,
                    Role.Author => AuthorAuthorization(task, memberName, context),
                    Role.Teacher => TeacherAuthorization(task, memberName, context),
                    Role.Student => StudentAuthorization(task, memberName, context),
                    _ => GuestAuthorization(task, memberName, context)
                };

        internal static bool AuthorAuthorization(Task task, string memberName, IContext context) =>
           task.IsAssignable() || task.AuthorId == Users.Me(context).Id;

        internal static bool TeacherAuthorization(Task task, string memberName, IContext context) =>
           task.IsAssignable() && 
            (IsTaskProperty(memberName) || 
                MatchesOneOf(
                    nameof(Task_Functions.AssignToAlInGroup), 
                    nameof(Task_Functions.AssignToIndividual)));

        internal static bool StudentAuthorization(Task task, string memberName, IContext context) =>
            TaskIsPublicOrAssignedToUser(task, context) && IsTaskProperty(memberName);

        internal static bool TaskIsPublicOrAssignedToUser(Task task, IContext context) =>
            task.IsPublic() || task.IsAssignedToCurrentUser(context);

        internal static bool GuestAuthorization(Task task, string memberName, IContext context) =>
            task.IsPublic() && IsTaskProperty(memberName) && !MatchesOneOf(memberName, nameof(Task.TeacherNotes));

        private static bool IsTaskProperty(string memberName) => IsProperty<Task>(memberName);
    }
}
