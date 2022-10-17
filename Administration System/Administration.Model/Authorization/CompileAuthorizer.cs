using Model.Functions.Services;
using NakedFunctions.Security;

namespace Model.Authorization; 

public class CompileAuthorizer : INamespaceAuthorizer {
    public bool IsVisible(object target, string memberName, IContext context) => true;
}