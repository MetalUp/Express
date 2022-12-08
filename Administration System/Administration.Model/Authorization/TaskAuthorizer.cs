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
                    Role.Author => IsTaskProperty(memberName) || UserIsAuthor(task, context),
                    _ => false
                };

        private static bool IsTaskProperty(string memberName) => IsProperty<Task>(memberName);

        private bool UserIsAuthor(Task task, IContext context) =>
            task.Project.AuthorId == Users.Me(context).Id;
    }
}
