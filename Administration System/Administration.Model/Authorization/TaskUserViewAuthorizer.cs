using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class TaskUserViewAuthorizer : ITypeAuthorizer<TaskUserView>
    {

        public bool IsVisible(TaskUserView tuv, string memberName, IContext context) =>
                Users.UserRole(context) switch
                {
                    Role.Root => true,
                    _ => TaskIsAssignedToUser(tuv, context)
                };

        internal static bool TaskIsAssignedToUser(TaskUserView tuv, IContext context) =>
            tuv.Project.IsAssignedToMe(context);
    }
}
