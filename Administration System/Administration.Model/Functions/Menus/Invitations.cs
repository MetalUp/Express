namespace Model.Functions.Menus
{
    public static class Invitations
    {
        public static (Invitation, IContext) InviteNewStudent(
           string name,
           [Optionally] Group group,
           IContext context)
        {
            (var student, var invite, var context2) = InviteNewUser(name, Role.Student, context);
            if (group != null)
            {
                var sg = new StudentGroup() { Student = student, Group = group };
                return (invite, context2.WithNew(sg));

            }
            else
            {
                return (invite, context);
            }
        }


        public static (Invitation, IContext) InviteNewTeacher(string name, IContext context)
        {
            (var teacher, var invite, var context2) = InviteNewUser(name, Role.Teacher, context);
            return (invite, context2);
        }

        private static (User, Invitation, IContext) InviteNewUser(
             string name,
             Role asRole,
             IContext context)
        {
            var me = Users.Me(context);
            var user = new User()
            {
                Name = name,
                Role = asRole,
                Status = UserStatus.PendingAcceptance,
                OrganisationId = me.OrganisationId,
                Organisation = me.Organisation
            };
            var invite = new Invitation()
            {
                Id = context.NewGuid(),
                Invitee = user,
                SenderId = me.Id,
                Sender = me,
                Sent = context.Now()
            };
            return (user, invite, context.WithNew(user).WithNew(invite));
        }

        public static IQueryable<Invitation> AllOurPendingInvitations(IContext context) => 
            context.Instances<Invitation>();


        public static IQueryable<Invitation> AllInvitations(IContext context) => context.Instances<Invitation>();

    }
}
