
using System.Linq;
using Model.Types;

namespace Model.Functions
{
    [Named("Users")]
    public static class User_MenuFunctions
    {
        public static (User, IContext) CreateNewUser(string friendlyName, IContext context)
        {
            var s = new User { FriendlyName = friendlyName };
            return (s, context.WithNew(s));
        }       

        public static IQueryable<User> AllUsers(IContext context) =>
            context.Instances<User>();

        public static IQueryable<User> FindByFriendlyName(string friendlyName,  IContext context) =>
            context.Instances<User>().Where(c => c.FriendlyName.ToUpper().Contains(friendlyName.ToUpper()));

        public static User FindByUserName(string userName, IContext context) =>
    context.Instances<User>().FirstOrDefault(c => c.UserName.ToUpper() == userName.ToUpper());


    }

}
