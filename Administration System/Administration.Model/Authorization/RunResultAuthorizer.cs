using NakedFunctions.Security;
using static Model.Authorization.Helpers;

namespace Model.Authorization
{
    public class RunResultAuthorizer : ITypeAuthorizer<RunResult>
    {

        public bool IsVisible(RunResult target, string memberName, IContext context)
        {
            return true;
        }
    }
}
