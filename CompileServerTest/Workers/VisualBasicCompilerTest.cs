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

    private readonly ILogger testLogger = NullLogger.Instance;

    [TestMethod]
    public void TestVersion() {
        var csv = VisualBasicCompiler.GetNameAndVersion();

        Assert.AreEqual("vb", csv[0]);
        Assert.AreEqual("16_9", csv[1]);
    }

    [TestMethod]
    public void TestVersionInParallel() {
        var csvs = Enumerable.Range(1, 10).AsParallel().Select(_ => VisualBasicCompiler.GetNameAndVersion()).ToArray();

        foreach (var csv in csvs) {
            Assert.AreEqual("vb", csv[0]);
            Assert.AreEqual("16_9", csv[1]);
        }
    }

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = VisualBasicRunSpec(SimpleCode);
        var (rr, code) = VisualBasicCompiler.Compile(runSpec, true);

        rr.AssertRunResult(Outcome.Ok);

        Assert.AreEqual(2560, code.Length);
    }

    [TestMethod]
    public void TestCompileAndRunOk() {
        var runSpec = VisualBasicRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1");
    }

    [TestMethod]
    public void TestCompileFailDivisionByZero() {
        var runSpec = VisualBasicRunSpec(DivZero);
        var (rr, code) = VisualBasicCompiler.Compile(runSpec, true);

        rr.AssertRunResult(Outcome.CompilationError, "(3) : error BC30542: Division by zero occurred while evaluating this expression.");
        Assert.AreEqual(0, code.Length);
    }

    [TestMethod]
    public void TestCompileAndRunFail() {
        var runSpec = VisualBasicRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value;

        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.RunTimeError, "", "", "Input string was not in a correct format.");
    }

    [TestMethod]
    public void TestCompileAndTestOk() {
        var runSpec = VisualBasicRunSpec(TestCodeOk);
        var rr = Handler.CompileAndTest(runSpec, testLogger).Result.Value;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestFail() {
        var runSpec = VisualBasicRunSpec(TestCodeFail);
        var rr = Handler.CompileAndTest(runSpec, testLogger).Result.Value;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Failed!  - Failed:     1, Passed:     0, Skipped:     0, Total:     1"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    //[TestMethod]
    //public void TestCompileAndRunInParallel() {
    //    var runSpecs = Enumerable.Range(1, 10).Select(i => VisualBasicRunSpec(SimpleCode));

    //    var rrs = runSpecs.AsParallel().Select(rr => Handler.CompileAndRun(rr, testLogger).Result.Value).ToArray();

    //    foreach (var rr in rrs) {
    //        Assert.IsNotNull(rr);
    //        Assert.AreEqual(Outcome.Ok, rr.outcome);
    //        Assert.AreEqual("", rr.cmpinfo);
    //        Assert.AreEqual("1", rr.stdout);
    //        Assert.AreEqual("", rr.stderr);
    //        Assert.AreEqual("", rr.run_id);
    //    }
    //}

    //[TestMethod]
    //public void TestCompileAndTestInParallel() {
    //    var runSpecs = Enumerable.Range(1, 10).Select(i => VisualBasicRunSpec(TestCodeOk));

    //    var rrs = runSpecs.AsParallel().Select(rr => Handler.CompileAndTest(rr, testLogger).Result.Value).ToArray();

    //    foreach (var rr in rrs) {
    //        Assert.IsNotNull(rr);
    //        Assert.AreEqual(Outcome.Ok, rr.outcome);
    //        Assert.AreEqual("", rr.cmpinfo);
    //        Assert.IsTrue(rr.stdout.Contains("Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1"), rr.stdout);
    //        Assert.AreEqual("", rr.stderr);
    //        Assert.AreEqual("", rr.run_id);
    //    }
    //}
}