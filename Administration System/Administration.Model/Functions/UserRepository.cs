
using System.Security.Cryptography;
using System.Text;

namespace Model.Functions
{
    public static class UserRepository
    {
        //TODO Can we cache this information?
        public static User Me(IContext context) =>
            context.Instances<User>().SingleOrDefault(c => c.UserName == HashedCurrentUserName(context));

        public static string HashedCurrentUserName(IContext context)
        {
            var userName = context.CurrentUser().Identity.Name;
            return userName == null ? "" : Hash(userName);
        }

        private static string Hash(string userName) =>
            SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(userName)).Aggregate("", (s, b) => s + b.ToString("x2"));


        public static User FindByUserName(string userName, IContext context) =>
    context.Instances<User>().FirstOrDefault(c => c.UserName == Hash(userName));

        public static (User, IContext) CreateNewPendingUser(string name, Role role, Organisation org, IContext context)
        {
            var s = new User { Name = name, Role = role, OrganisationId = org.Id, Organisation = org, Status = UserStatus.PendingAcceptance };
            return (s, context.WithNew(s));
        }



        public static bool UserHasRoleAtLeast(Role role, IContext context) => UserRole (context) >= role;

        public static bool UserHasSpecificRole(Role role, IContext context) => UserRole(context) == role;

        public static Role UserRole(IContext context)
        {
            var user = Me(context);
            return user == null ? Role.Guest : user.Role;
        }
    }
}
