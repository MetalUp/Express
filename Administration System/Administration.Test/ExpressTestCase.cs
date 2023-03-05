using NakedFramework.RATL.TestCase;

namespace Test;

public abstract class ExpressTestCase : BaseRATLNUnitTestCase {
    protected Home GetHome() => ROSIApi.GetHome(new Uri("http://localhost/"), TestInvokeOptions()).Result;

    protected DomainObject GetObject<T>(string key) => GetObject(FullName<T>(), key);
}