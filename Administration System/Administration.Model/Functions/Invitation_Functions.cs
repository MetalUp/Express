
namespace Model.Functions
{
    public static class Invitation_Functions
    {
        public static (User, IContext) Accept(this Invitation invitation, IContext context) {
            var student = invitation.Invitee;
            var student_U = new User(student) { Status = UserStatus.Active };
            return (student_U, context.WithUpdated(student, student_U).WithDeleted(invitation));
        }

        public static (User, IContext) Cancel(this Invitation invitation, IContext context)
        {
            var student = invitation.Invitee;
            var student_U = new User(invitation.Invitee) { Status = UserStatus.Inactive };
            return (student_U, context.WithUpdated(student, student_U).WithDeleted(invitation));
        }

    }
}
