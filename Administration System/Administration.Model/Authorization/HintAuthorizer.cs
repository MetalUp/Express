using NakedFunctions.Security;

namespace Model.Authorization
{
    public class HintAuthorizer : ITypeAuthorizer<Hint>
    {

        public bool IsVisible(Hint hint, string memberName, IContext context) =>
            Users.UserRole(context) switch
            {
                Role.Root => true,
                Role.Author => true,
                Role.Teacher => Helpers.MemberIsProperty(hint, memberName),
                Role.Student => TaskAuthorizer.TaskIsPublicOrAssignedToUser(hint.Task, context) && 
                                Helpers.MemberIsProperty(hint, memberName),
                _ => hint.Task.IsPublic() &&  Helpers.MemberIsProperty(hint, memberName)
            };

    }
}
