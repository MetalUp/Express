
namespace Model.Functions
{
    public static class User_Functions
    {
        #region display of User

        public static bool HideEmailAddress(this User user) => user.Role < Role.Teacher;

        #endregion

        #region Editing
        [Edit]
        public static IContext EditName(
            this User student,
            string name,
            IContext context) =>
            context.WithUpdated(student, new(student) { Name = name });
        #endregion



        #region End of lifecycle
        public static IContext SetToInactive(this User user, IContext context) =>
            context.WithUpdated(user, new User(user) { Status = UserStatus.Inactive });

        public static string DisableSetToInactive(this User user, IContext context) =>
            user.Status == UserStatus.PendingAcceptance ? "Outstanding Invitation must first be cancelled" : null;

        public static bool HideSetToInactive(this User user, IContext context) =>
           user.Status == UserStatus.Inactive;

        public static IContext RemoveIndentityInfo(this User user,
            [DescribedAs("type REMOVE IDENTITY")] string confirm,
            IContext context) =>
            context.WithUpdated(user, new User(user) { UserName = "", Name = "" });

        public static string ValidateRemoveIndentityInfo(this User user,
            string confirm,
            IContext context) =>
             confirm.ToUpper() == "REMOVE IDENTITY" ? null : "Must type REMOVE IDENTITY into the Confirm field";

        public static bool HideRemoveIndentityInfo(this User user,
            string confirm,
            IContext context) =>
            user.Status != UserStatus.Inactive;

        #endregion


        #region internal methods
        internal static bool HasRoleAtLeast(this User user, Role role) => (int) user.Role >= (int)role;

        internal static bool HasRole(this User user, Role role) => user.Role == role;
        #endregion

        #region Assignments

        [PageSize(20)]
        public static IQueryable<Assignment> Assignments(this User user, IContext context) => 
            Menus.Assignments.AssignmentsTo(user, context);

        public static IContext AssignTask(this User user, Task task, DateTime dueBy, IContext context) =>
            Menus.Assignments.NewAssignmentToIndividual(user, task, dueBy, context);

        #endregion

        #region Invitation
        public static (Invitation, IContext) CreateNewInvitation(this User toUser, User sender, IContext context)
        {
            var inv = new Invitation()
            {
                InviteeId = toUser.Id,
                Invitee = toUser,
                SenderId = sender.Id,
                Sender = sender,
                Sent = context.Now()
            };
            return (inv, context.WithNew(inv));
        }
        #endregion

    }
}
