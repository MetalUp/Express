using NakedFunctions.Security;

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
                    Role.Teacher  => TeacherAuthorization(task, memberName, context),
                    Role.Student => StudentAuthorization(task, memberName, context),
                    _ => GuestAuthorization(task, memberName, context)
                };

        private bool AuthorAuthorization(Task task, string memberName, IContext context) => 
           task.IsAssignable() || task.AuthorId == Users.Me(context).Id;

        private bool TeacherAuthorization(Task task, string memberName, IContext context) => 
           task.IsAssignable() && (IsGuestVisibleProperty(memberName) || Helpers.MatchesOneOf(nameof(Task.TeacherNotes)));

        private bool StudentAuthorization(Task task, string memberName, IContext context) =>
            (task.IsPublic() || task.IsAssignedToCurrentUser(context)) && IsGuestVisibleProperty(memberName);

        private bool GuestAuthorization(Task task, string memberName, IContext context) =>
            task.IsPublic() && IsGuestVisibleProperty(memberName);

        private bool IsGuestVisibleProperty(string memberName) =>
            Helpers.MatchesOneOf(memberName,
                nameof(Task.Link),
                nameof(Task.Status),
                nameof(Task.Title),
                nameof(Task.Language),
                nameof(Task.MaxMarks));
    }
}
