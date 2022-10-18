using CompileServer.Models;
using CompileServer.Workers;

namespace CompileServerTest.Workers;

[TestClass]
public class CSharpCompilerTest {
    private const string SimpleCode = "var a = 1;";
    private const string DivZero = "var a = 1/0;";

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = new RunSpec { language_id = "csharp", sourcecode = SimpleCode };
        var (rr, code) = CSharpCompiler.Compile(runSpec);

        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.AreEqual("", rr.run_id);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.stdout);

        Assert.AreEqual(2048, code.Length);
    }

    [TestMethod]
    public void TestCompileFailDivisionByZero()
    {
        var runSpec = new RunSpec { language_id = "csharp", sourcecode = DivZero };
        var (rr, code) = CSharpCompiler.Compile(runSpec);

        Assert.AreEqual(Outcome.CompilationError, rr.outcome);
        Assert.AreEqual("CS0020: Division by constant zero", rr.cmpinfo);
        Assert.AreEqual("", rr.run_id);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.stdout);

        Assert.AreEqual(0, code.Length);
    }
}