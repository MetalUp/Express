using NakedFunctions.Security;

namespace Model.Authorization
{
    public class TaskAuthorViewAuthorizer : ITypeAuthorizer<TaskAuthorView>
    {
        public bool IsVisible(TaskAuthorView task, string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                Role.Author =>true,
                _ => false,
            };
    }
}
