using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class HintUserViewAuthorizer : ITypeAuthorizer<HintUserView>
    {

        public bool IsVisible(HintUserView huv, string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                Role.Author => true,
                _ => IsForATaskAssignedToUser(huv, context)
            };

        private static bool IsForATaskAssignedToUser(HintUserView huv, IContext context) =>
            Tasks.GetTask(huv.TaskId, context).Project.IsAssignedToMe(context);
    }
}
