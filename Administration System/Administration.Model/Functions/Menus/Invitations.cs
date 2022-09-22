namespace Model.Functions.Menus
{
    public static class Invitations
    {
        public static (User, IContext) InviteNewStudent(
           string name,
           [Optionally] Group group,
           IContext context)
        {
            (var student, var context2) = InviteNewUser(name, Role.Student, Organisations.MyOrganisation(context), context);
            if (group != null)
            {
                var sg = new StudentGroup() { Student = student, Group = group };
                return (student, context2.WithNew(sg));

            }
            else
            {
                return (student, context);
            }
        }


        public static (User, IContext) InviteNewTeacher(string name, IContext context)
        {
            (var teacher, var context2) = InviteNewUser(name, Role.Teacher, Organisations.MyOrganisation(context), context);
            return (teacher, context2);
        }

        public static (User, IContext) InviteNewUser(
             string name,
             Role asRole,
             Organisation organisation,
             IContext context)
        {
            var user = new User()
            {
                Name = name,
                InvitationCode = context.NewGuid().ToString(),
                Role = asRole,
                Status = UserStatus.PendingAcceptance,
                OrganisationId = organisation.Id,
                Organisation = organisation
            };
            return (user, context.WithNew(user));
        }

        public static (User, IContext) AcceptInvitation(
            [RegEx("^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$")]
            [DescribedAs("Paste your Invitation Code here")] string code,
            IContext context)
        {
            var userName = context.CurrentUser().Identity.Name;
            var invitee = context.Instances<User>().Single(u => u.InvitationCode == code);
            var invitee2 = new User(invitee) {UserName = userName, InvitationCode = null, Status = UserStatus.Active };
            return (invitee2, context.WithUpdated(invitee, invitee2));
        }

        public static string ValidateAcceptInvitation(string code, IContext context) =>
            context.Instances<User>().Any(u => u.InvitationCode == code) ? null :
            "That is not a valid Invitation Code";
    }
}
