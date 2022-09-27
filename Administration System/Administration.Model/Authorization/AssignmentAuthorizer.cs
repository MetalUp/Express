using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class AssignmentAuthorizer : ITypeAuthorizer<Assignment>
    {
        public bool IsVisible(Assignment a, string memberName, IContext context)
        {
            var me = Users.Me(context);
            return me.Role   switch
            {
                Role.Root => true,
                >= Role.Teacher => TeacherAuthorization(a, memberName,me, context),
                Role.Student => StudentAuthorization(a, memberName, me, context),
                _ => false
            };
        }

        private static bool TeacherAuthorization(Assignment a, string memberName, User me, IContext context) =>
            IsAssignedToMe(a, me) ||
            IsAssignedByOrToAnyoneInMyOrganisation(a, me) && !MatchesOneOf(memberName, nameof(Assignment.Link));
            
        private static bool StudentAuthorization(Assignment a, string memberName, User me, IContext context) =>
            IsAssignedToMe(a, me) && IsProperty<Assignment>(memberName);

        private static bool IsAssignedToMe(Assignment a, User me) => a.AssignedToId == me.Id;

        private static bool IsAssignedByOrToAnyoneInMyOrganisation(Assignment a, User me) =>
            a.AssignedBy.OrganisationId == me.OrganisationId ||
            a.AssignedTo.OrganisationId == me.OrganisationId;
    }

}
