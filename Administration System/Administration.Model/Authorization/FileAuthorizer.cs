using NakedFunctions.Security;
using System.Security.Principal;

namespace Model.Authorization
{
    public class FileAuthorizer : ITypeAuthorizer<File>
    {
        public bool IsVisible(File target, string memberName, IContext context)
        {
            return true; //TODO
        }
    }
}
