using NakedFunctions.Security;

namespace Model.Authorization
{
    public class TaskAuthorizer : ITypeAuthorizer<Task>
    {
        //TODO: Student must not be able to access the content files of a task directly;
        // so only author has access to methods to retrieve (or save) a content file associated with a task

        public bool IsVisible(Task task, string memberName, IContext context) =>
            task.IsPublic() && Helpers.MemberIsProperty(task, memberName) ? true :
                UserRepository.UserRole(context) switch
                {
                    Role.Root => true,
                    Role.Author => AuthorAuthorization(task, memberName, context),
                    Role.Teacher  => TeacherAuthorization(task, memberName, context),
                    Role.Student => StudentAuthorization(task, memberName, context),
                    _ => false
                };

        private bool AuthorAuthorization(Task task, string memberName, IContext context) => 
           task.IsAssignable() || task.AuthorId == UserRepository.Me(context).Id;

        private bool TeacherAuthorization(Task task, string memberName, IContext context) => 
           task.IsAssignable() && (!(memberName.StartsWith("Edit") || memberName.StartsWith("Add")));

        private bool StudentAuthorization(Task task, string memberName, IContext context) =>
            task.IsAssignedToCurrentUser(context) && Helpers.MemberIsProperty(task, memberName);
    }
}
