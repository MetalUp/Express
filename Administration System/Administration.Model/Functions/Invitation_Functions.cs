
namespace Model.Functions
{
    public static class Invitation_Functions
    {
        public static (User, IContext) Accept(this Invitation inv, IContext context) {
            var student = inv.Invitee;
            var student_U = new User(student) {
                UserName = Users.HashedCurrentUserName(context), 
                Status = UserStatus.Active };
            return (student_U, context.WithUpdated(student, student_U).WithDeleted(inv));
        }

        public static (User, IContext) Cancel(this Invitation inv, IContext context)
        {
            var student = inv.Invitee;
            var student_U = new User(inv.Invitee) { Status = UserStatus.Inactive };
            return (student_U, context.WithUpdated(student, student_U).WithDeleted(inv));
        }

        internal static bool IsPending(this Invitation invitation) =>
            invitation.Invitee.Status == UserStatus.PendingAcceptance;
    }
}
