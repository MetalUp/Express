using Model.Functions;

namespace Model.Functions.Menus
{

    public static class Root
    {
        #region Users
        [MemberOrder(1)]
        public static User Me(IContext context) => UserRepository.Me(context);

        [MemberOrder(2)]
        public static User FindByUserName(string userName, IContext context) => UserRepository.FindByUserName(userName, context);

        [MemberOrder(3)]
        public static IQueryable<User> AllUsers(IContext context) => context.Instances<User>();

        [CreateNew]
        public static (User, IContext) CreateNewPendingUser(string name, Role role, Organisation org, IContext context) =>
            UserRepository.CreateNewPendingUser(name, role, org, context);

        public static IQueryable<User> AllStudents(IContext context) => 
            context.Instances<User>().Where(u => u.Role == Role.Teacher);

        public static IQueryable<User> AllTeachers(IContext context) =>  
            context.Instances<User>().Where(u => u.Role == Role.Teacher);
        #endregion

        #region Organisations
        public static IQueryable<Organisation> AllOrganisations(IContext context) => context.Instances<Organisation>();

        [CreateNew]
        public static (Organisation, IContext) CreateNewOrganisation(string name, IContext context)
        {
            var org = new Organisation() { Name = name };
            return (org, context.WithNew(org));
        }
        #endregion

        #region Groups
        public static IQueryable<Group> AllGroups(IContext context) => context.Instances<Group>();

        [CreateNew]
        public static (Group, IContext) CreateNewGroup(string name, IContext context)
        {
            var g = new Group() { Name = name };
            return (g, context.WithNew(g));
        }
        #endregion

        #region Tasks
        public static IQueryable<Task> AllTasks(IContext context) => context.Instances<Task>();

        [CreateNew]
        public static (Task, IContext) CreateNewTask(string title, IContext context)
        {
            var t = new Task() { Title = title, AuthorId = Teachers.Me(context).Id };
            return (t, context.WithNew(t));
        }
        #endregion

        #region Assignments
        public static IQueryable<Assignment> AllAssignments(IContext context) => context.Instances<Assignment>();

        #endregion

        #region Activities
        public static IQueryable<Activity> AllActivities(IContext context) => context.Instances<Activity>();

        #endregion

        #region Invitations
        public static IQueryable<Invitation> AllInvitations(IContext context) => context.Instances<Invitation>();

        public static (Invitation, IContext) CreateNewInvitation(User toUser, IContext context) =>
             toUser.CreateNewInvitation(UserRepository.Me(context), context);

        #endregion


    }
}
