

namespace Model.Functions
{
    [Named("Users")]
    public static class Users_Menu
    {
        public static IQueryable<User> AllUsers(IContext context) => 
           context.Instances<User>();

        [CreateNew] //TODO: attribute does not appear to be working
        public static (User, IContext) CreateNewUser(
             [RegEx(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}")] string userName, 
            [Password] string password, //TODO add Regex or additional validation methods for valid password
            [Password] string confirmPassword,
            IContext context)
        {
            var s = new User { UserName = userName, Password = password };
            return (s, context.WithNew(s));
        }

        public static string ValidateCreateNewUser(
             string userName, string password, string confirmPassword, IContext context) =>
              context.Instances<User>().Any(u => u.UserName.ToUpper() == userName.ToUpper()) ?
                    "User name already exists - Log In instead" :
                        password == confirmPassword ? "" : "Passwords do not match.";
            
        public static IQueryable<User> FindByFriendlyName(string friendlyName,  IContext context) =>
            context.Instances<User>().Where(c => c.FullName.ToUpper().Contains(friendlyName.ToUpper()));

        public static User Me(IContext context) => context.Instances<User>().Single(u => u.Id == 3); //TODO: This is mocked out while there is no log on. Will actually find user by the username of the Principal (obtained from the context)

        public static User FindByUserName(string userName, IContext context) =>
    context.Instances<User>().FirstOrDefault(c => c.UserName.ToUpper() == userName.ToUpper());




    }

}
