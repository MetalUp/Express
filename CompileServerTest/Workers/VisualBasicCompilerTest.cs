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

    private const string TestCodeOk =
        @"
        Imports Microsoft.VisualStudio.TestTools.UnitTesting

        Namespace ExpressTests
            <TestClass>
            Public Class Tests
                <TestMethod>
                Public Sub ATest()
                    Assert.IsTrue(True)
                End Sub
            End Class
        End Namespace";

    private const string TestCodeFail =
        @"
        Imports Microsoft.VisualStudio.TestTools.UnitTesting

        Namespace ExpressTests
            <TestClass>
            Public Class Tests
                <TestMethod>
                Public Sub ATest()
                    Assert.IsTrue(False)
                End Sub
            End Class
        End Namespace";

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

    [TestMethod]
    public void TestCompileAndTestOk()
    {
        var runSpec = VisualBasicRunSpec(TestCodeOk);
        var rr = Handler.CompileAndTest(runSpec).Result.Value;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestFail()
    {
        var runSpec = VisualBasicRunSpec(TestCodeFail);
        var rr = Handler.CompileAndTest(runSpec).Result.Value;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Failed!  - Failed:     1, Passed:     0, Skipped:     0, Total:     1"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }
}