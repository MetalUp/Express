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
                    Role.Author => IsProperty<Task>(memberName) || UserIsAuthor(task, context),
                    _ => false
                };

        private bool UserIsAuthor(Task task, IContext context) =>
            task.Project.AuthorId == Users.Me(context).Id;
    }
}
