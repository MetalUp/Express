using NakedFunctions.Security;

namespace Model.Authorization
{
    public class TaskAuthorViewAuthorizer : ITypeAuthorizer<TaskAuthorView>
    {
        //TODO: Student must not be able to access the content files of a task directly;
        // so only author has access to methods to retrieve (or save) a content file associated with a task

        public bool IsVisible(TaskAuthorView task, string memberName, IContext context) =>
                Users.UserRole(context) switch
                {
                    Role.Root => true,
                    Role.Author =>true,
                    _ => false,
                };
    }
}
