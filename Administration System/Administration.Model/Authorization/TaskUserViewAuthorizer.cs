using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class TaskUserViewAuthorizer : ITypeAuthorizer<TaskUserView>
    {

        public bool IsVisible(TaskUserView tuv, string memberName, IContext context) => true;
        //Note that authorization is now controlled separately in the TaskAccess service
                

    }
}
