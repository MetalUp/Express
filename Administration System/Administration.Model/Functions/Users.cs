
using System.Security.Cryptography;
using System.Text;

namespace Model.Functions
{
    public static class Users
    {
        public static User Me(IContext context)
        {
            var userName = Hash(context.CurrentUser().Identity.Name);
            //TODO: use encryption
            return context.Instances<User>().SingleOrDefault(c => c.UserName == userName);
        }

        public static User FindByUserName(string userName, IContext context) =>
    context.Instances<User>().FirstOrDefault(c => c.UserName == Hash(userName));
        //TODO: use encryption


        public static string Hash(string userName) =>
            SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(userName)).Aggregate("", (s, b) => s + b.ToString("x2"));


    }

}
