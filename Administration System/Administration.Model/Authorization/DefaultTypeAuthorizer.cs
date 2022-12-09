using Model.Functions;
using NakedFunctions.Security;

namespace Model.Authorization
{

    public class DefaultTypeAuthorizer : ITypeAuthorizer<object>
    {
        public bool IsVisible(object target, string memberName, IContext context)
            => throw new Exception($"Default Type Authorizer is being called for {target.GetType().Name}. Add a specific authorizer");
    }
}
