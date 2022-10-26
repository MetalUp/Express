using CompileServer.Controllers;
using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using static CompileServerTest.TestHelpers;

namespace CompileServerTest.Workers;

[TestClass]
public class PythonCompilerTest {
    private readonly ILogger testLogger = NullLogger.Instance;


    private const string SimpleCode =
        @"print (str(1))";

    private const string MissingTerm =
        @"print (str(1/))";

    private const string RunTimeFail =
        @"print (int(""invalid""))";

    private const string TestCodeOk =
@"def test_Ok():
    assert sum([1, 2, 3]) == 6, ""Should be 6""

if __name__ == ""__main__"":
    test_Ok()
    print(""Everything passed"")
";

    private const string TestCodeFail =
@"def test_Fail():
    assert sum([1, 2, 4]) == 6, ""Should be 6""

if __name__ == ""__main__"":
    test_Fail()
    print(""Everything passed"")
";

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
        CompileServerController.PythonPath = "C:\\Python310";
    }

    [TestMethod]
    public void TestVersion() {
        var csv = PythonCompiler.GetNameAndVersion();

        Assert.AreEqual("python", csv[0]);
        Assert.IsTrue(csv[1].StartsWith("3.10."));
    }

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = PythonRunSpec(SimpleCode);
        var (rr, file) = PythonCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.Ok);

        Assert.AreEqual("temp.py", file);
    }

    [TestMethod]
    public void TestCompileAndRunOk() {
        var runSpec = PythonRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1\r\n");
    }

    [TestMethod]
    public void TestCompileFailMissingTerm() {
        var runSpec = PythonRunSpec(MissingTerm);
        var (rr, file) = PythonCompiler.Compile(runSpec);

        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"File""{Path.GetTempPath()}temp.py"",line1print(str(1/))^SyntaxError:invalidsyntax");

        Assert.AreEqual("temp.py", file);
    }

    [TestMethod]
    public void TestCompileAndRunFail()
    {
        var runSpec = PythonRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value;

        Assert.IsNotNull(rr);
        rr.stderr = ClearWhiteSpace(rr.stderr);

        rr.AssertRunResult(Outcome.RunTimeError, "", "", @$"Traceback(mostrecentcalllast):File""{Path.GetTempPath()}temp.py"",line1,in<module>print(int(""invalid""))ValueError:invalidliteralforint()withbase10:'invalid'");
    }

    [TestMethod]
    public void TestCompileAndTestOk()
    {
        var runSpec = PythonRunSpec(TestCodeOk);
        var rr = Handler.CompileAndTest(runSpec, testLogger).Result.Value;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Everything passed"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestFail()
    {
        var runSpec = PythonRunSpec(TestCodeFail);
        var rr = Handler.CompileAndTest(runSpec, testLogger).Result.Value;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.RunTimeError, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.AreEqual("", rr.stdout);
        Assert.IsTrue(rr.stderr.Contains("Should be 6"), rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }
}