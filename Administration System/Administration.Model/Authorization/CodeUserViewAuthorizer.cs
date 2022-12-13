using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class CodeUserViewAuthorizer : ITypeAuthorizer<CodeUserView>
    {

        public bool IsVisible(CodeUserView target, string memberName, IContext context)
        {
            return true;
        }
    }
}
