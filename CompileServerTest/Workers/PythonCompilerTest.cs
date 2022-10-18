using CompileServer.Controllers;
using CompileServer.Models;
using CompileServer.Workers;

namespace CompileServerTest.Workers;

[TestClass]
public class PythonCompilerTest {

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
        CompileServerController.PythonPath = "C:\\Python310";
    }

    private const string SimpleCode =
        @"print (str(1))";

    private const string MissingTerm =
        @"print (str(1/))";

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = Helpers.PythonRunSpec(SimpleCode);
        var (rr, code) = PythonCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.Ok);

        Assert.AreEqual("temp.py", code);
    }

    [TestMethod]
    public void TestCompileFailMissingTerm()
    {
        var runSpec = Helpers.PythonRunSpec(MissingTerm);
        var (rr, code) = PythonCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.CompilationError, "  File \"C:\\Users\\scasc\\AppData\\Local\\Temp\\temp.py\", line 1\r\n    print (str(1/))\r\n                 ^\r\nSyntaxError: invalid syntax\r\n");

        Assert.AreEqual("temp.py", code);
    }
}