
using System.Security.Cryptography;
using System.Text;

namespace Model.Menus
{
    public static class Users
    {
        [MemberOrder(10)]
        public static User Me(IContext context) =>
            context.Instances<User>().SingleOrDefault(c => c.UserName == HashedCurrentUserName(context));

        internal static User FindById(int uid, IContext context) =>
            context.Instances<User>().Single(u => u.Id == uid);

        [MemberOrder(20)]
        public static User FindByUserName(string userName, IContext context) =>
    context.Instances<User>().SingleOrDefault(c => c.UserName == Hash(userName));

        [TableView(false, nameof(User.Name), nameof(User.Role), nameof(User.Organisation))]
        public static IQueryable<User> AllUsers(IContext context) =>context.Instances<User>();

        #region Students & Colleagues
        [TableView(false, nameof(User.Name), nameof(User.Role), nameof(User.EmailAddress))]
        internal static IQueryable<User> OurUsers(IContext context)
        {
            int myOrgId = Me(context).OrganisationId;
            return context.Instances<User>().Where(s => s.OrganisationId == myOrgId).
                OrderBy(s => s.Name);
        }

        [MemberOrder(100)]
        [TableView(false, nameof(User.Name), nameof(User.Groups))]
        public static IQueryable<User> OurStudents(IContext context) =>
            OurUsers(context).Where(u => u.Role == Role.Student);

        [MemberOrder(120)]
        public static User FindStudentByName(User student, IContext context) => student;

        public static IQueryable<User> AutoComplete0FindStudentByName(string nameOrPartName, IContext context) => 
            OurStudents(context).Where(u => u.Name.ToUpper().Contains(nameOrPartName.ToUpper()));

        [MemberOrder(130)]
        [TableView(false, nameof(User.Name), nameof(User.EmailAddress))]
        public static IQueryable<User> MyColleagues(IContext context)
        {
            var myId = Me(context).Id;
            return OurUsers(context).Where(t => t.Id != myId);
        }
        #endregion

        #region private & internal
        internal static string HashedCurrentUserName(IContext context)
        {
            var userName = context.CurrentUser().Identity.Name;
            return userName == null ? "" : Hash(userName);
        }

        private static string Hash(string userName) =>
            SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(userName)).Aggregate("", (s, b) => s + b.ToString("x2"));

        internal static bool UserHasRoleAtLeast(Role role, IContext context) => UserRole(context) >= role;

        internal static bool UserHasSpecificRole(Role role, IContext context) => UserRole(context) == role;

        internal static Role UserRole(IContext context)
        {
            var user = Me(context);
            return user == null ? Role.Guest : user.Role;
        }
        #endregion
    }
}
