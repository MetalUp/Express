using NakedFramework.RATL.TestCase;

namespace Test;

public abstract class ExpressTestCase : BaseRATLNUnitTestCase {
    protected Home GetHome() => ROSIApi.GetObject(new Uri("http://localhost/"), type, id, TestInvokeOptions()).Result;

    protected DomainObject GetObject<T>(string key) => GetObject(FullName<T>(), key);
}