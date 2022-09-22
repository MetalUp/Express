
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
        public static IQueryable<User> Students(IContext context)
        {
            int myOrgId = Me(context).OrganisationId;
            return context.Instances<User>().Where(s => s.OrganisationId == myOrgId).
                OrderBy(s => s.Name);
        }

        public static IQueryable<User> StudentsPendingAcceptance(IContext context) =>
            Students(context).Where(u => u.Status == UserStatus.PendingAcceptance);

        public static User FindStudentByLoginId(string userName, IContext context) =>
            Students(context).SingleOrDefault(c => c.UserName == Hash(userName));

        public static IQueryable<User> FindStudentByName(string nameOrPartName, IContext context) =>
            Students(context).Where(u => u.Name.ToUpper().Contains(nameOrPartName.ToUpper()));

        public static IQueryable<User> MyColleagues(IContext context)
        {
            var me = Me(context);
            int myOrgId = me.OrganisationId;
            int myId = me.Id;
            return context.Instances<User>().Where(t => t.OrganisationId == myOrgId && t.Id != myId);
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
