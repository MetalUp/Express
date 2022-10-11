using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class HintAuthorizer : ITypeAuthorizer<Hint>
    {

        public bool IsVisible(Hint hint, string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                Role.Author => true,
                Role.Teacher => IsHintProperty(memberName),
                Role.Student => IsForATaskAssignedToUser(hint, context) && 
                               IsHintProperty(memberName),
                _ => false
            };

        private static bool IsHintProperty(string memberName) => IsProperty<Hint>(memberName);

        private static bool IsForATaskAssignedToUser(Hint hint, IContext context) =>
            hint.Tasks.Any(t => TaskAuthorizer.TaskIsDefaultOrAssignedToUser(t, context));

    }
}
