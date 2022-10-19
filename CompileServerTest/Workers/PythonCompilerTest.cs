using CompileServer.Controllers;
using CompileServer.Models;
using CompileServer.Workers;

namespace CompileServerTest.Workers;

[TestClass]
public class PythonCompilerTest {
    private const string SimpleCode =
        @"print (str(1))";

    private const string MissingTerm =
        @"print (str(1/))";

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
        CompileServerController.PythonPath = "C:\\Python310";
    }

    [TestMethod]
    public void TestVersion() {
        var csv = PythonCompiler.GetNameAndVersion();

        Assert.AreEqual("python", csv[0]);
        Assert.AreEqual("3.10.2", csv[1]);
    }

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = TestHelpers.PythonRunSpec(SimpleCode);
        var (rr, code) = PythonCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.Ok);

        Assert.AreEqual("temp.py", code);
    }

    [TestMethod]
    public void TestCompileFailMissingTerm() {
        var runSpec = TestHelpers.PythonRunSpec(MissingTerm);
        var (rr, code) = PythonCompiler.Compile(runSpec);

        rr.cmpinfo = TestHelpers.ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"File""{Path.GetTempPath()}temp.py"",line1print(str(1/))^SyntaxError:invalidsyntax");

        Assert.AreEqual("temp.py", code);
    }
}