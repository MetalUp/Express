
namespace Model.Functions
{
    public static class User_Functions
    {
        #region Editing
        [Edit]
        public static IContext EditName(
            this User student,
            string name,
            IContext context) =>
            context.WithUpdated(student, new(student) { Name = name });

        [Edit]
        public static IContext EditEmailAddress(
            this User student,
            string emailAddress,
            IContext context) =>
            context.WithUpdated(student, new(student) { EmailAddress = emailAddress });

        public static bool HideEditEmailAddress(
            this User student,
            string emailAddress,
            IContext context) =>
            student.HasRole(Role.Student);
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

        #region Grouping
        public static IContext AddToGroup(this User student, Group group, IContext context) =>
                group.AddStudent(student, context);

        public static List<Group> Choices1AddToGroup(this User student, Group group, IContext context) =>
            Groups.AllOurGroups(context).ToList();

        public static IContext AddSelectedStudentsToGroup(this IQueryable<User> students, Group group, IContext context) =>
            students.Aggregate(context, (c, s) => group.AddStudent(s, c));

        public static List<Group> Choices1AddSelectedStudentsToGroup(this IQueryable<User> students, Group group, IContext context) =>
            Groups.AllOurGroups(context).ToList();
        #endregion
    }
}
