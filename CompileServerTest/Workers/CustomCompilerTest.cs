using System.Globalization;
using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static CompileServerTest.TestHelpers;

namespace CompileServerTest.Workers;

[TestClass]
public class CustomCompilerTest {
    private const string SimpleCode =
        @"main
var a = 1
end main
";

    private const string CompileFail =
        @"main
var a = 1
endmain
";

   

    private readonly ILogger testLogger = NullLogger.Instance;

    [ClassInitialize]
    public static void Initialize(TestContext testContext) { }

    [TestInitialize]
    public void Initialize() {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
    }

   

    [TestMethod, Ignore]
    public void TestCompileOk() {
        using var runSpec = CustomRunSpec(SimpleCode);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok);
    }


    [TestMethod, Ignore]
    public void TestCompileFail() {
        using var runSpec = CustomRunSpec(CompileFail);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.CompilationError, "Error: line 3:0 extraneous input 'endmain' expecting {'end main', 'var'}\r\n");
    }
  
}