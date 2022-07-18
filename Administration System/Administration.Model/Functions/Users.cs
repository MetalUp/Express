

namespace Model.Functions
{
    [Named("Users")]
    public static class User_MenuFunctions
    {
        public static IQueryable<User> AllUsers(IContext context) => 
           context.Instances<User>();

        public static (User, IContext) CreateNewUser(string friendlyName, IContext context)
        {
            var s = new User { FriendlyName = friendlyName };
            return (s, context.WithNew(s));
        }       
    
        public static IQueryable<User> FindByFriendlyName(string friendlyName,  IContext context) =>
            context.Instances<User>().Where(c => c.FriendlyName.ToUpper().Contains(friendlyName.ToUpper()));

        public static User Me(IContext context) => context.Instances<User>().Single(u => u.Id == 3); //TODO: This is mocked out while there is no log on. Will actually find user by the username of the Principal (obtained from the context)

        public static User FindByUserName(string userName, IContext context) =>
    context.Instances<User>().FirstOrDefault(c => c.UserName.ToUpper() == userName.ToUpper());

     
        #region All Students
        public static IQueryable<User> MyStudents(IContext context)
        {
            int myOrgId = Me(context).OrganisationId;
            return context.Instances<User>().Where(u => u.OrganisationId == myOrgId && u.Role == Role.Student);
        }

        public static IQueryable<User> MyTeacherColleagues(IContext context)
        {
            User me = Me(context);
            int myOrgId = me.OrganisationId;
            int myId = me.Id;
            return context.Instances<User>().Where(u => u.OrganisationId == myOrgId && u.Role == Role.Teacher && u.Id != myId);
        }

            #endregion

    }

}
