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

        private static string GenerateInvitationCode(IContext context) => context.NewGuid().ToString();

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


    }
}
