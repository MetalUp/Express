using NakedFunctions.Security;

namespace Model.Authorization
{
    public class AssignmentAuthorizer : ITypeAuthorizer<Assignment>
    {
        public bool IsVisible(Assignment a, string memberName, IContext context)
        {
            var me = Users.Me(context);
            return me.Role   switch
            {
                >= Role.Teacher => TeacherAuthorization(a, memberName,me, context),
                Role.Student => StudentAuthorization(a, memberName, me, context),
                _ => false
            };
        }

        private bool TeacherAuthorization(Assignment a, string memberName, User me, IContext context) =>
            a.AssignedById == me.Id ||
            a.AssignedBy.OrganisationId == me.OrganisationId ||
            a.AssignedTo.OrganisationId == me.OrganisationId;

        private bool StudentAuthorization(Assignment a, string memberName, User me, IContext context) =>
            a.AssignedToId == me.Id && Helpers.MemberIsProperty(a, memberName);
    }

}
