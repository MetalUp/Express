
namespace Model.Functions
{
    public static class Invitation_Functions
    {
        public static (User, IContext) Accept(this Invitation inv, IContext context) {
            var student = inv.Invitee;
            var student_U = new User(student) {
                UserName = UserRepository.HashedCurrentUserName(context), 
                Status = UserStatus.Active };
            return (student_U, context.WithUpdated(student, student_U));
        }

        public static (User, IContext) Cancel(this Invitation invitation, IContext context)
        {
            throw new NotImplementedException();
            //var student = invitation.Invitee;
            //var student_U = new User(invitation.Invitee) { Status = UserStatus.Inactive };
            //return (student_U, context.WithUpdated(student, student_U).WithDeleted(invitation));
        }

        internal static bool IsPending(this Invitation invitation) =>
            invitation.Invitee.Status == UserStatus.PendingAcceptance;
    }
}
