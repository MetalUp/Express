using CompileServer.Models;
using CompileServer.Workers;
using static CompileServerTest.TestHelpers;

namespace CompileServerTest.Workers;

[TestClass]
public class VisualBasicCompilerTest {
    private const string SimpleCode =
        @"Module Program
          Sub Main(args As String())       
            Dim a = 1
            System.Console.Write(a)
            End Sub 
          End Module";

    private const string DivZero =
        @"Module Program
          Sub Main(args As String())       
            Dim a = 1 \ 0
            End Sub 
          End Module";

    private const string RunTimeFail =
        @"Module Program
          Sub Main(args As String())       
            Dim a = Integer.Parse(""invalid"") 
            End Sub 
          End Module";

    [TestMethod]
    public void TestVersion() {
        var csv = VisualBasicCompiler.GetNameAndVersion();

        Assert.AreEqual("vb", csv[0]);
        Assert.AreEqual("16_9", csv[1]);
    }

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = VisualBasicRunSpec(SimpleCode);
        var (rr, code) = VisualBasicCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.Ok);

        Assert.AreEqual(2560, code.Length);
    }

    [TestMethod]
    public void TestCompileAndRunOk() {
        var runSpec = VisualBasicRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec).Result.Value;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1");
    }

    [TestMethod]
    public void TestCompileFailDivisionByZero() {
        var runSpec = VisualBasicRunSpec(DivZero);
        var (rr, code) = VisualBasicCompiler.Compile(runSpec);

        rr.AssertRunResult(Outcome.CompilationError, "(3) : error BC30542: Division by zero occurred while evaluating this expression.");
        Assert.AreEqual(0, code.Length);
    }

    [TestMethod]
    public void TestCompileAndRunFail() {
        var runSpec = VisualBasicRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec).Result.Value;

        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.RunTimeError, "", "", "Input string was not in a correct format.");
    }
}