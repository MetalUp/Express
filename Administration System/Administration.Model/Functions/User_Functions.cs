
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
            string confirm,
            IContext context) =>
            context.WithUpdated(user, new User(user) { UserName = "", Name = "" });

        public static string ValidateRemoveIndentityInfo(this User user,
            string confirm,
            IContext context) =>
             confirm.ToUpper() == "CONFIRM" ? null : "Must type CONFIRM into the Confirm field";

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
        public static IQueryable<Assignment> Assignments(this User student, IContext context)
        {
            var studentId = student.Id;
            return context.Instances<Assignment>().Where(a => a.AssignedToId == studentId).OrderByDescending(a => a.DueBy);
        }

        public static (Assignment, IContext) AssignTask(this User student, Task task, DateTime dueBy, IContext context)
        {
            if (task.Status == TaskStatus.UnderDevelopment) throw new Exception("Tasks under development cannot be assigned - this should have been prevented");
            var a = new Assignment()
            {
                AssignedToId = student.Id,
                AssignedById = UserRepository.Me(context).Id,
                TaskId = task.Id,
                DueBy = dueBy,
                Marks = 0,
                Status = AssignmentStatus.PendingStart
            };
            return (a, context.WithNew(a));
        }
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
