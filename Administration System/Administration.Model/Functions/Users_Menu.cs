

namespace Model.Functions
{
    [Named("Users")]
    public static class Users_Menu
    {


        public static User Me(IContext context)
        {
            var userName = context.CurrentUser().Identity.Name;
            return context.Instances<User>().SingleOrDefault(c => c.UserName == userName);
        }

        public static User FindByUserName(string userName, IContext context) =>
    context.Instances<User>().FirstOrDefault(c => c.UserName.ToUpper() == userName.ToUpper());


    }

}
