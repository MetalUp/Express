
using System.Security.Cryptography;
using System.Text;

namespace Model.Functions
{
    public static class Users
    {
        public static User Me(IContext context)
        {
            var userName = context.CurrentUser().Identity.Name;
            var hash = userName == null ? "" : Hash(userName);
            return context.Instances<User>().SingleOrDefault(c => c.UserName == hash);
        }

        public static User FindByUserName(string userName, IContext context) =>
    context.Instances<User>().FirstOrDefault(c => c.UserName == Hash(userName));

        public static (User, IContext) CreateNewUser(string userName, Role role, IContext context)
        {
            var s = new User { UserName = Hash(userName), Role = role };
            return (s, context.WithNew(s));
        }

        public static string Hash(string userName) =>
            SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(userName)).Aggregate("", (s, b) => s + b.ToString("x2"));


    }

}
