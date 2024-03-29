﻿using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class ActivityAuthorizer : ITypeAuthorizer<Activity>
    {
        public bool IsVisible(Activity act, string memberName, IContext context)
        {
            var me = Users.Me(context);
            var assignId = act.AssignmentId;
            var assgn = context.Instances<Assignment>().SingleOrDefault(a => a.Id == assignId);
            return me.Role   switch
            {
                Role.Root => true,
                >= Role.Teacher => TeacherAuthorization(assgn, memberName, me, context),
                Role.Student => StudentAuthorization(assgn, memberName, me, context),
                _ => false
            };
        }

        internal static bool TeacherAuthorization(Assignment assgn, string memberName, User me, IContext context) =>
            assgn is not null &&
            assgn.IsAssignedByOrToAnyoneInOrganisation(me.Organisation) && IsProperty<Activity>(memberName);

        internal static bool StudentAuthorization(Assignment assgn, string memberName, User me, IContext context) =>
            assgn is not null &&
            assgn.IsAssignedTo(me) && IsProperty<Activity>(memberName);

    }
}