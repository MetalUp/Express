using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static CompileServerTest.TestHelpers;

namespace CompileServerTest.Workers;

[TestClass]
public class VisualBasicCompilerTest {
    private const string SimpleCode =
        @"Imports Microsoft.VisualStudio.TestTools.UnitTesting
          Module Program
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

    private const string StackOverFlowFail =
        @"Module Program
            Private Function F(ByVal i As Integer) As Integer
                Return F(i + 1)
            End Function

            Sub Main(args As String())
                Dim a = F(1)   
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

    private const string TestCodeStackOverflow =
        @"
        Imports Microsoft.VisualStudio.TestTools.UnitTesting

        Namespace ExpressTests
            <TestClass>
            Public Class Tests
                <TestMethod>
                Public Sub ATest()
                    ATest()
                End Sub
            End Class
        End Namespace";

    private readonly ILogger testLogger = NullLogger.Instance;

    [TestMethod]
    public void TestVersion() {
        var csv = Handler.GetNameAndVersion(VisualBasicRunSpec(""));

        Assert.AreEqual("vb", csv[0]);
        Assert.AreEqual("16_9", csv[1]);
    }

    [TestMethod]
    public void TestVersionInParallel() {
        var csvs = Enumerable.Range(1, 10).AsParallel().Select(_ => Handler.GetNameAndVersion(VisualBasicRunSpec(""))).ToArray();

        foreach (var csv in csvs) {
            Assert.AreEqual("vb", csv[0]);
            Assert.AreEqual("16_9", csv[1]);
        }
    }

    [TestMethod]
    public void TestCompileOk() {
        using var runSpec = VisualBasicRunSpec(SimpleCode);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok);
    }

    [TestMethod]
    public void TestCompileAndRunOk() {
        using var runSpec = VisualBasicRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1");
    }

    [TestMethod]
    public void TestCompileFailDivisionByZero() {
        using var runSpec = VisualBasicRunSpec(DivZero);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.CompilationError, "(3) : error BC30542: Division by zero occurred while evaluating this expression.");
        Assert.AreEqual(3, rr.line_no);
        Assert.AreEqual(21, rr.col_no);
    }

    [TestMethod]
    public void TestCompileAndRunFail() {
        using var runSpec = VisualBasicRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;

        Assert.IsNotNull(rr);
        rr.AssertRunResultContains(Outcome.RunTimeError, "", "", "Input string was not in a correct format.");
    }

    [TestMethod]
    public void TestCompileAndTestOk() {
        using var runSpec = VisualBasicRunSpec(TestCodeOk);
        var rr = Handler.CompileAndTest(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestFail() {
        using var runSpec = VisualBasicRunSpec(TestCodeFail);
        var rr = Handler.CompileAndTest(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Failed!  - Failed:     1, Passed:     0, Skipped:     0, Total:     1"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndRunStackOverflow() {
        using var runSpec = VisualBasicRunSpec(StackOverFlowFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResultContains(Outcome.RunTimeError, "", "", "Stack overflow");
    }

    //[TestMethod]
    //public void TestCompileAndTestStackOverflow() {
    //    using var runSpec = VisualBasicRunSpec(TestCodeStackOverflow);
    //    var rr = Handler.CompileAndTest(runSpec, testLogger).Result.Value as RunResult;
    //    Assert.IsNotNull(rr);
    //    Assert.AreEqual(Outcome.RunTimeError, rr.outcome);
    //    Assert.AreEqual("", rr.cmpinfo);
    //    Assert.IsTrue(rr.stderr.Contains("Stack overflow"));
    //    //Assert.IsTrue(rr.stdout.Contains("Failed!  - Failed:     1, Passed:     0, Skipped:     0, Total:     1"), rr.stdout);
    //    Assert.AreEqual("", rr.run_id);
    //}

    [TestMethod]
    public void TestCompileAndRunInParallel() {
        var runSpecs = Enumerable.Range(1, 10).Select(_ => VisualBasicRunSpec(SimpleCode));

        var rrs = runSpecs.AsParallel().Select(rr => Handler.CompileAndRun(rr, testLogger).Result.Value).Cast<RunResult>().ToArray();

        foreach (var rr in rrs) {
            Assert.IsNotNull(rr);
            Assert.AreEqual(Outcome.Ok, rr.outcome);
            Assert.AreEqual("", rr.cmpinfo);
            Assert.AreEqual("1", rr.stdout);
            Assert.AreEqual("", rr.stderr);
            Assert.AreEqual("", rr.run_id);
        }

        foreach (var testRunSpec in runSpecs) {
            testRunSpec.Dispose();
        }
    }

    [TestMethod]
    public void TestCompileAndTestInParallel() {
        var runSpecs = Enumerable.Range(1, 10).Select(_ => VisualBasicRunSpec(TestCodeOk));

        var rrs = runSpecs.AsParallel().Select(rr => Handler.CompileAndTest(rr).Result.Value).Cast<RunResult>().ToArray();

        foreach (var rr in rrs) {
            Assert.IsNotNull(rr);
            Assert.AreEqual(Outcome.Ok, rr.outcome);
            Assert.AreEqual("", rr.cmpinfo);
            Assert.IsTrue(rr.stdout.Contains("Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1"), rr.stdout);
            Assert.AreEqual("", rr.stderr);
            Assert.AreEqual("", rr.run_id);
        }

        foreach (var testRunSpec in runSpecs) {
            testRunSpec.Dispose();
        }
    }
}