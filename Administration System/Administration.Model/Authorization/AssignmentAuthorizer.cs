using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class AssignmentAuthorizer : ITypeAuthorizer<Assignment>
    {
        public bool IsVisible(Assignment assgn, string memberName, IContext context)
        {
            var me = Users.Me(context);
            return me.Role   switch
            {
                Role.Root => true,
                >= Role.Teacher => TeacherAuthorization(assgn, memberName,me, context),
                Role.Student => StudentAuthorization(assgn, memberName, me, context),
                _ => false
            };
        }

        private static bool TeacherAuthorization(Assignment assgn, string memberName, User me, IContext context) =>
            assgn.IsAssignedTo(me) ||
            assgn.IsAssignedByOrToAnyoneInOrganisation(me.Organisation) && !MatchesOneOf(memberName, nameof(Assignment.Link));
            
        private static bool StudentAuthorization(Assignment assgn, string memberName, User me, IContext context) =>
            assgn.IsAssignedTo(me) && IsProperty<Assignment>(memberName);
    }
}
