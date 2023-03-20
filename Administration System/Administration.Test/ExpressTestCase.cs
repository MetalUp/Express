using System.Security.Principal;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.RATL.TestCase;
using User = Model.Types.User;

namespace Test;

public abstract class ExpressTestCase : BaseRATLNUnitTestCase {
    protected static IIdentity TestIdentity { get; } = new GenericIdentity("");

    protected new IPrincipal TestPrincipal { get; set; } = new GenericPrincipal(TestIdentity, new string[] { });
    protected Home GetHome() => ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;

    protected DomainObject GetObject<T>(string key) => GetObject(FullName<T>(), key);

    protected void LogInAs(string name) {
        var user = ServiceScope.ServiceProvider.GetService<IObjectPersistor>()?.Instances<User>().SingleOrDefault(u => u.Name == name);
        TestPrincipal = new GenericPrincipal(new GenericIdentity(user?.UserName ?? ""), new string[] { });
    }
}