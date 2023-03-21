using System.Security.Principal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Framework;
using NakedFramework.RATL.TestCase;
using User = Model.Types.User;

namespace Test;

public abstract class ExpressTestCase : BaseRATLNUnitTestCase {
  
    protected static IIdentity TestIdentity { get; } = new GenericIdentity("");

    protected new IPrincipal TestPrincipal { get; set; } = new GenericPrincipal(TestIdentity, new string[] { });
    protected Home GetHome() => ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;

    protected DomainObject GetObject<T>(string key) => GetObject(FullName<T>(), key);

    protected void LogInAs(string name) {
        // the login name must be the identity name eg google-oauth2|10........
        TestPrincipal = new GenericPrincipal(new GenericIdentity(name), new string[] { });
    }

    protected override IConfigurationBuilder AddUserSecrets(IConfigurationBuilder configBuilder) => configBuilder.AddUserSecrets<ExpressTestCase>();
}