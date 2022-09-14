using Model.Functions;
using Model.Types;
using NakedFramework.Metamodel.Authorization;
using NakedFunctions;
using NakedFunctions.Reflector.Authorization;
using NakedFunctions.Security;
using Model.Authorization;

namespace Server
{
    public static class AuthorizationHelpers
    {
        public static IAuthorizationConfiguration AdminAuthConfig()
        {
            var config = new AuthorizationConfiguration<DefaultTypeAuthorizer, DefaultMainMenuAuthorizer>();
            //config.AddNamespaceAuthorizer<MyAppAuthorizer>("MyApp");
            //config.AddNamespaceAuthorizer<MyCluster1Authorizer>("MyApp.MyCluster1");
            //config.AddTypeAuthorizer<Bar, MyBarAuthorizer>();
            return config;
        }


    }

}
