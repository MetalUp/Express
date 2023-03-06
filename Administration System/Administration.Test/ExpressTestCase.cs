using System.Security.Principal;
using NakedFramework.Architecture.Framework;
using NakedFramework.Persistor.EFCore.Util;
using NakedFramework.RATL.TestCase;
using User = Model.Types.User;

namespace Test;

public abstract class ExpressTestCase : BaseRATLNUnitTestCase {
    protected Home GetHome() => ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;

    protected DomainObject GetObject<T>(string key) => GetObject(FullName<T>(), key);

    protected static IIdentity TestIdentity { get; } = new GenericIdentity("");

    protected new IPrincipal TestPrincipal { get; set; } = new GenericPrincipal(TestIdentity, new string[]{});

    protected void LogInAs(string name) {
        TestPrincipal = new GenericPrincipal(new GenericIdentity(name), new String [] { });
    }


}