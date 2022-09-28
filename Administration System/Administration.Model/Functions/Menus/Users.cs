
using System.Security.Cryptography;
using System.Text;

namespace Model.Functions.Menus
{
    public static class Users
    {
        [MemberOrder(10)]
        public static User Me(IContext context) =>
            context.Instances<User>().SingleOrDefault(c => c.UserName == HashedCurrentUserName(context));

        [MemberOrder(20)]
        public static User FindByUserName(string userName, IContext context) =>
    context.Instances<User>().SingleOrDefault(c => c.UserName == Hash(userName));

        [MemberOrder(30)]
        public static (User, IContext) CreateNewPendingUser(string name, Role role, Organisation org, IContext context)
        {
            var s = new User { Name = name, Role = role, OrganisationId = org.Id, Organisation = org, Status = UserStatus.PendingAcceptance };
            return (s, context.WithNew(s));
        }

        public static IQueryable<User> AllUsers(IContext context) =>context.Instances<User>();

        #region Students & Colleagues
        private static IQueryable<User> OurUsers(IContext context)
        {
            int myOrgId = Me(context).OrganisationId;
            return context.Instances<User>().Where(s => s.OrganisationId == myOrgId).
                OrderBy(s => s.Name);
        }

        [MemberOrder(100)]
        public static IQueryable<User> OurStudents(IContext context) =>
            OurUsers(context).Where(u => u.Role == Role.Student);

        [MemberOrder(110)]
        public static IQueryable<User> StudentsPendingAcceptance(IContext context) =>
            OurStudents(context).Where(u => u.Status == UserStatus.PendingAcceptance);

        [MemberOrder(120)]
        public static User FindStudentByName(User student, IContext context) => student;

        public static IQueryable<User> AutoComplete0FindStudentByName(string nameOrPartName, IContext context) => 
            OurStudents(context).Where(u => u.Name.ToUpper().Contains(nameOrPartName.ToUpper()));

        [MemberOrder(130)]
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
