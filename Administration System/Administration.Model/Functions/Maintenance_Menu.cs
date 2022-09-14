
namespace Model.Functions
{
    [Named("Maintenance")]
    public static class Maintenance_Menu
    {
        #region Users
        public static IQueryable<User> AllUsers(IContext context) => context.Instances<User>();

        [CreateNew] //TODO: attribute does not appear to be working
        public static (User, IContext) CreateNewUser(
     [RegEx(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}")] string userName,
    IContext context)
        {
            var s = new User { UserName = userName };
            return (s, context.WithNew(s));
        }
        #endregion

        #region Students
        public static IQueryable<Student> AllStudents(IContext context) => context.Instances<Student>();

        public static (Student, IContext) CreateNewStudent(User fromUser,  string name, Organisation organisation, IContext context)
        {
            var s = new Student() { UserId = fromUser.Id, Name = name, OrganisationId = organisation.Id };           
            return (s, context.WithNew(s).WithUpdated(fromUser, new User(fromUser) { Role = Role.Student}));
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
            var org= new Organisation() { Name = name};
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
            var  t = new Task() { Title = title, AuthorId = Teachers_Menu.Me(context).Id };
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
