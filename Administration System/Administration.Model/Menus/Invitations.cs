using System.Security.Cryptography;
using System.Text;

namespace Model.Menus
{
    public static class Invitations
    {
        public static (User, IContext) InviteNewStudentInMyOrganisation(
           string name,
           [Optionally] Group group,
           IContext context)
        {
            (var student, var context2) = InviteNewUserToSpecifiedOrganisation(name, Role.Student, Organisations.MyOrganisation(context), context);
            if (group != null)
            {
                return (student, Group_Functions.AddStudent(group, student, context));
            }
            else
            {
                return (student, context2);
            }
        }

        public static IList<Group> Choices1InviteNewStudentInMyOrganisation(IContext context) =>  Groups.AllGroups(context).ToList();

        public static (User, IContext) InviteNewTeacherInMyOrganisation(string name, IContext context)
        {
            (var teacher, var context2) = InviteNewUserToSpecifiedOrganisation(name, Role.Teacher, Organisations.MyOrganisation(context), context);
            return (teacher, context2);
        }

        internal static string GenerateInvitationCode(IContext context) => context.NewGuid().ToString();

        public static (User, IContext) InviteNewUserToSpecifiedOrganisation(
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

        public static IQueryable<User> OutstandingInvitationsForMyOrganisation(IContext context) =>
            Users.OurUsers(context).Where(u => u.Status == UserStatus.PendingAcceptance);


        public static IQueryable<User> AllOutstandingInvitations(IContext context) =>
            Users.AllUsers(context).Where(u => u.Status == UserStatus.PendingAcceptance);

    }
}
