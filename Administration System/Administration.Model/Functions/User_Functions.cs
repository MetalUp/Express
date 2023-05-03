
namespace Model.Functions
{
    public static class User_Functions
    {
        #region Display
        public static bool HideLinkToBeEmailed(this User user) => user.InvitationCode is null || user.InvitationCode == "";

        public static bool HideEmailAddress(this User user) => user.Role == Role.Student; //Email addresses are deliberately not displayed (or stored) for students
        #endregion

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

        public static bool HideEditEmailAddress(this User student) => student.HasRole(Role.Student);
        #endregion

        #region End of lifecycle
        public static IContext SetToInactive(this User user, IContext context) =>
            context.WithUpdated(user, new User(user) { Status = UserStatus.Inactive });

        public static string DisableSetToInactive(this User user, IContext context) =>
            user.Status == UserStatus.PendingAcceptance ? "Outstanding Invitation must first be cancelled" : null;

        public static bool HideSetToInactive(this User user, IContext context) =>
           user.Status == UserStatus.Inactive;

        public static IContext RemoveIdentityInfo(this User user,
            [DescribedAs("type REMOVE IDENTITY")] string confirm,
            IContext context) =>
            context.WithUpdated(user, new User(user) { UserName = "", Name = "" });

        public static string ValidateRemoveIdentityInfo(this User user, string confirm) =>
             confirm.ToUpper() == "REMOVE IDENTITY" ? null : "Must type REMOVE IDENTITY into the Confirm field";

        public static bool HideRemoveIdentityInfo(this User user) =>  user.Status != UserStatus.Inactive;

        #endregion

        #region internal methods
        internal static bool HasRoleAtLeast(this User user, Role role) => (int) user.Role >= (int)role;

        internal static bool HasRole(this User user, Role role) => user.Role == role;
        #endregion

        public static IContext InviteToChangeLoginCredentials(
            this User user,
            IContext context)
        {
            var user2 = new User(user)
            {
                InvitationCode = Invitations.GenerateInvitationCode(context),
                UserName = null
            };
            return context.WithUpdated(user, user2);
        }

        #region Assignments

        [PageSize(20)]
        public static IQueryable<Assignment> RecentAssignments(this User user, IContext context) => 
            Menus.Assignments.AssignmentsTo(user, context);
        #endregion

        #region Grouping
        public static IContext AddToGroup(this User student, Group group, IContext context) =>
               context.WithUpdated(student,
                new User(student) { Groups = student.Groups.Append(group).ToList() });

        public static List<Group> Choices1AddToGroup(this User student, Group group, IContext context) =>
            Groups.AllOurGroups(context).ToList();

        public static IContext AddSelectedStudentsToGroup(this IQueryable<User> students, Group group, IContext context) =>
            students.Aggregate(context, (c, s) => s.AddToGroup(group, c)); 

        public static List<Group> Choices1AddSelectedStudentsToGroup(this IQueryable<User> students, IContext context) =>
            Groups.AllOurGroups(context).ToList();
        #endregion

        #region Administrator actions

        public static IContext ChangeRole(
            this User student,
            Role role,
            IContext context) =>
            context.WithUpdated(student, new(student) { Role = role });

        public static IContext ChangeOrganisation(
            this User student,
            Organisation newOrg,
            IContext context) =>
            context.WithUpdated(student, new(student) { OrganisationId = newOrg.Id, Organisation = newOrg });

       public static IContext DeleteUser(this User user, [DescribedAs("type DELETE")] string confirm, IContext context) =>
            context.WithDeleted(user);

      public static string ValidateDelete(this User user, string confirm) =>
            confirm == "DELETE" ? null : "Must type 'DELETE'";

        #endregion

        #region Activities
        public static IQueryable<Activity> RecentActivity(this User user, IContext context) =>
            Activities.RecentActivityForUser(user, context);
        #endregion
    }
}
