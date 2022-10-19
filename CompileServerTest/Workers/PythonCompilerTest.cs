using CompileServer.Controllers;
using CompileServer.Models;
using CompileServer.Workers;
using static CompileServerTest.TestHelpers;

namespace CompileServerTest.Workers;

[TestClass]
public class PythonCompilerTest {
    private const string SimpleCode =
        @"print (str(1))";

    private const string MissingTerm =
        @"print (str(1/))";

    private const string RunTimeFail =
        @"print (int(""invalid""))";

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
        var rr = Handler.CompileAndRun(runSpec).Result.Value;
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
        var rr = Handler.CompileAndRun(runSpec).Result.Value;

        Assert.IsNotNull(rr);
        rr.stderr = ClearWhiteSpace(rr.stderr);

        rr.AssertRunResult(Outcome.RunTimeError, "", "", @$"Traceback(mostrecentcalllast):File""{Path.GetTempPath()}temp.py"",line1,in<module>print(int(""invalid""))ValueError:invalidliteralforint()withbase10:'invalid'");
    }
}