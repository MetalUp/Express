
using System.Security.Cryptography;
using System.Text;

namespace Model.Functions
{
    public static class UserRepository
    {
        //TODO Can we cache this information?
        public static User Me(IContext context)
        {
            var userName = context.CurrentUser().Identity.Name;
            var hash = userName == null ? "" : Hash(userName);
            return context.Instances<User>().SingleOrDefault(c => c.UserName == hash);
        }

        public static User FindByUserName(string userName, IContext context) =>
    context.Instances<User>().FirstOrDefault(c => c.UserName == Hash(userName));

        public static (User, IContext) CreateNewUser(string userName, Role role, Organisation org, IContext context)
        {
            var s = new User { UserName = Hash(userName), Role = role, OrganisationId = org.Id, Organisation = org };
            return (s, context.WithNew(s));
        }

        public static string Hash(string userName) =>
            SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(userName)).Aggregate("", (s, b) => s + b.ToString("x2"));


        public static bool UserHasRoleAtLeast(Role role, IContext context) => UserRole (context) >= role;

        public static bool UserHasSpecificRole(Role role, IContext context) => UserRole(context) == role;

        public static Role UserRole(IContext context)
        {
            var user = Me(context);
            return user == null ? Role.Guest : user.Role;
        }
    }
}
