using CompileServer.Models;
using CompileServer.Workers;

namespace CompileServerTest.Workers;

[TestClass]
public class CSharpCompilerTest {
    private const string SimpleCode = "var a = 1;";
    private const string DivZero = "var a = 1/0;";

    [TestMethod]
    public void TestVersion() {
        var csv = CSharpCompiler.GetNameAndVersion();

        Assert.AreEqual("csharp", csv[0]);
        Assert.AreEqual("10", csv[1]);
    }

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = TestHelpers.CsharpRunSpec(SimpleCode);
        var (rr, code) = CSharpCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.Ok);
        Assert.AreEqual(2048, code.Length);
    }

    [TestMethod]
    public void TestCompileFailDivisionByZero() {
        var runSpec = TestHelpers.CsharpRunSpec(DivZero);
        var (rr, code) = CSharpCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.CompilationError, "(1,9): error CS0020: Division by constant zero");
        Assert.AreEqual(0, code.Length);
    }
}