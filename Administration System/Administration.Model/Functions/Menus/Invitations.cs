using System.Security.Cryptography;
using System.Text;

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
                return (student, Group_Functions.AddStudent(group, student, context));
            }
            else
            {
                return (student, context2);
            }
        }

        public static (User, IContext) InviteNewTeacher(string name, IContext context)
        {
            (var teacher, var context2) = InviteNewUser(name, Role.Teacher, Organisations.MyOrganisation(context), context);
            return (teacher, context2);
        }

        private static string GenerateInvitationCode(IContext context) =>
            SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(context.RandomSeed().Value.ToString()))
            .Aggregate("", (s, b) => s + b.ToString("x2"));

        public static (User, IContext) InviteNewUser(
             string name,
             Role asRole,
             Organisation organisation,
             IContext context)
        {
            var user = new User()
            {
                Name = name,
                InvitationCode = GenerateInvitationCode(context),
                Role = asRole,
                Status = UserStatus.PendingAcceptance,
                OrganisationId = organisation.Id,
                Organisation = organisation
            };
            return (user, context.WithNew(user));
        }

        public static (User, IContext) AcceptInvitation(
            [DescribedAs("Paste your Invitation Code here")] string code,
            IContext context)
        {
            var userName = Users.HashedCurrentUserName(context);
            var invitee = context.Instances<User>().Single(u => u.InvitationCode == code);
            var invitee2 = new User(invitee) {UserName = userName, InvitationCode = null, Status = UserStatus.Active };
            return (invitee2, context.WithUpdated(invitee, invitee2));
        }

        public static string ValidateAcceptInvitation(string code, IContext context) =>
            context.Instances<User>().Any(u => u.InvitationCode == code) ? null :
            "That is not a valid Invitation Code";
    }
}
