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
                Role.Student => TaskAuthorizer.TaskIsPublicOrAssignedToUser(hint.Task, context) && 
                               IsHintProperty(memberName),
                _ => hint.Task.IsPublic() && IsHintProperty(memberName)
            };

        private static bool IsHintProperty(string memberName) => IsProperty<Hint>(memberName);

    }
}
