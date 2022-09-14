using Model.Functions;

namespace Model.Functions.Menus
{

    public static class Maintenance
    {
        #region Users
        [MemberOrder(1)]
        public static User Me(IContext context) => Users.Me(context);

        [MemberOrder(2)]
        public static User FindByUserName(string userName, IContext context) => Users.FindByUserName(userName, context);

        [MemberOrder(3)]
        public static IQueryable<User> AllUsers(IContext context) => context.Instances<User>();

        public static (User, IContext) CreateNewUser(string userName, Role role, IContext context) =>
            Users.CreateNewUser(userName, role, context);
        #endregion

        #region Students
        public static IQueryable<Student> AllStudents(IContext context) => context.Instances<Student>();

        public static (Student, IContext) CreateNewStudent(User fromUser, string name, Organisation organisation, IContext context)
        {
            var s = new Student() { UserId = fromUser.Id, Name = name, OrganisationId = organisation.Id };
            return (s, context.WithNew(s).WithUpdated(fromUser, new User(fromUser) { Role = Role.Student }));
        }

        #endregion

        #region Teachers
        public static IQueryable<Teacher> AllTeachers(IContext context) => context.Instances<Teacher>();

        [CreateNew]
        public static (Teacher, IContext) CreateNewTeacher(User fromUser, string name, Organisation organisation, IContext context)
        {
            var t = new Teacher() { UserId = fromUser.Id, Name = name, OrganisationId = organisation.Id };
            return (t, context.WithNew(t).WithUpdated(fromUser, new User(fromUser) { Role = Role.Teacher }));
        }
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



    }
}
