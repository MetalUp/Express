

namespace Model.Functions
{
    public static class Users
    {
        public static User Me(IContext context)
        {
            var userName = context.CurrentUser().Identity.Name;
            //TODO: use encryption
            return context.Instances<User>().SingleOrDefault(c => c.UserName == userName);
        }

        public static User FindByUserName(string userName, IContext context) =>
    context.Instances<User>().FirstOrDefault(c => c.UserName.ToUpper() == userName.ToUpper());
        //TODO: use encryption
    }

}
