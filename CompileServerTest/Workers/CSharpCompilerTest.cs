using CompileServer.Models;
using CompileServer.Workers;
using static CompileServerTest.TestHelpers;

namespace CompileServerTest.Workers;

[TestClass]
public class CSharpCompilerTest {
    private const string SimpleCode = "var a = 1;";
    private const string DivZero = "var a = 1/0;";
    private const string RunTimeFail = @"var a = int.Parse(""invalid"");";

    [TestMethod]
    public void TestVersion() {
        var csv = CSharpCompiler.GetNameAndVersion();

        Assert.AreEqual("csharp", csv[0]);
        Assert.AreEqual("10", csv[1]);
    }

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = CsharpRunSpec(SimpleCode);
        var (rr, code) = CSharpCompiler.Compile(runSpec);
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok);
        Assert.AreEqual(2048, code.Length);
    }

    [TestMethod]
    public void TestCompileAndRunOk() {
        var runSpec = CsharpRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec).Result.Value;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok);
    }

    [TestMethod]
    public void TestCompileFailDivisionByZero() {
        var runSpec = CsharpRunSpec(DivZero);
        var (rr, code) = CSharpCompiler.Compile(runSpec);
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.CompilationError, "(1,9): error CS0020: Division by constant zero");
        Assert.AreEqual(0, code.Length);
    }

    [TestMethod]
    public void TestCompileAndRunFail() {
        var runSpec = CsharpRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec).Result.Value;

        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.RunTimeError, "", "", "Input string was not in a correct format.");
    }
}