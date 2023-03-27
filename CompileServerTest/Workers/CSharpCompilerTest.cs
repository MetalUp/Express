using System.Collections.Immutable;
using System.Text;
using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static CompileServerTest.TestHelpers;

namespace CompileServerTest.Workers;

[TestClass]
public class CSharpCompilerTest {
    private const string SimpleCode = @"
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    var a = 1;System.Console.Write(a);";

    private const string SimpleCodeWithChar = @"
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    System.Console.Write(""\u25A0"");";

    private const string SimpleCodeWithMain = @"
    public class Wrapper {
        static void Main() {
            System.Console.WriteLine(""1"");
        }
    }";

    private const string DivZero = "var a = 1/0;";

    private const string Infinity = @"
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    var a = System.Double.PositiveInfinity;System.Console.Write(a);";

    private const string RunTimeFail = @"var a = int.Parse(""invalid"");";

    private const string StackOverFlowFail = @"static int f(int i) => f(i+1);var a = f(1);";

    private const string TestCodeOk =
        @"
        using Microsoft.VisualStudio.TestTools.UnitTesting;

        namespace ExpressTests;

        [TestClass]
        public class Tests
        {
            [TestMethod]
            public void ATest()
            {
                Assert.IsTrue(true);
            }
        }";

    private const string TestCodeFail =
        @"
        using Microsoft.VisualStudio.TestTools.UnitTesting;

        namespace ExpressTests;

        [TestClass]
        public class Tests
        {
            [TestMethod]
            public void ATest()
            {
                Assert.IsTrue(false);
            }
        }";

    private const string TestCodeRTE =
        @"
        using Microsoft.VisualStudio.TestTools.UnitTesting;

        namespace ExpressTests;

        [TestClass]
        public class Tests
        {
            [TestMethod]
            public void ATest()
            {
                ((object)null).ToString();
            }
        }";

    private const string TestCodeStackOverflow =
        @"
        using Microsoft.VisualStudio.TestTools.UnitTesting;

        namespace ExpressTests;

        [TestClass]
        public class Tests
        {
            [TestMethod]
            public void ATest()
            {
                ATest();
            }
        }";

    private const string CodeWithImmutable = @"
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Immutable;
    using System.Linq;
    var a = ImmutableList.Create<int>(1); System.Console.Write(a.First());";

    private readonly ILogger testLogger = NullLogger.Instance;

    [TestMethod]
    public void TestVersion() {
        var runSpec = CsharpRunSpec("");
        var csv = Handler.GetNameAndVersion(runSpec);

        Assert.AreEqual("csharp", csv[0]);
        Assert.AreEqual("10", csv[1]);
    }

    [TestMethod]
    public void TestVersionInParallel() {
        var csvs = Enumerable.Range(1, 10).AsParallel().Select(_ => Handler.GetNameAndVersion(CsharpRunSpec(""))).ToArray();

        foreach (var csv in csvs) {
            Assert.AreEqual("csharp", csv[0]);
            Assert.AreEqual("10", csv[1]);
        }
    }

    [TestMethod]
    public void TestCompileOk() {
        using var runSpec = CsharpRunSpec(SimpleCode);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok);
        var a = ImmutableList.Create<int>(1);
    }

    [TestMethod]
    public void TestCompileOkWithMain() {
        using var runSpec = CsharpRunSpec(SimpleCodeWithMain);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok);
    }

    [TestMethod]
    public void TestCompileAndRunOk() {
        using var runSpec = CsharpRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1");
    }

    [TestMethod]
    public void TestCompileAndRunInfinity() {
        using var runSpec = CsharpRunSpec(Infinity);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "∞");
    }

    [TestMethod]
    public void TestCompileAndRunOkWithChar() {
        using var runSpec = CsharpRunSpec(SimpleCodeWithChar);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "■");
    }

    [TestMethod]
    public void TestCompileAndRunImmutable() {
        using var runSpec = CsharpRunSpec(CodeWithImmutable);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1");
    }


    [TestMethod]
    public void TestCompileFailDivisionByZero() {
        using var runSpec = CsharpRunSpec(DivZero);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.CompilationError, "(1,9): error CS0020: Division by constant zero");
        Assert.AreEqual(1, rr.line_no);
        Assert.AreEqual(9, rr.col_no);
    }

    [TestMethod]
    public void TestCompileAndRunFail() {
        using var runSpec = CsharpRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;

        Assert.IsNotNull(rr);
        rr.AssertRunResultContains(Outcome.RunTimeError, "", "", "Input string was not in a correct format.");
    }

    [TestMethod]
    public void TestCompileAndRunStackOverflow() {
        using var runSpec = CsharpRunSpec(StackOverFlowFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;

        Assert.IsNotNull(rr);
        rr.AssertRunResultContains(Outcome.RunTimeError, "", "", "Stack overflow");
    }

    [TestMethod]
    public void TestCompileAndTestOk() {
        using var runSpec = CsharpRunSpec(TestCodeOk);
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
        using var runSpec = CsharpRunSpec(TestCodeFail);
        var rr = Handler.CompileAndTest(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Failed!  - Failed:     1, Passed:     0, Skipped:     0, Total:     1"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestRTE() {
        using var runSpec = CsharpRunSpec(TestCodeRTE);
        var rr = Handler.CompileAndTest(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Failed!  - Failed:     1, Passed:     0, Skipped:     0, Total:     1"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    //[TestMethod]
    //public void TestCompileAndTestStackOverflow() {
    //    using var runSpec = CsharpRunSpec(TestCodeStackOverflow);
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
        var runSpecs = Enumerable.Range(1, 10).Select(_ => CsharpRunSpec(SimpleCode));

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
        var runSpecs = Enumerable.Range(1, 10).Select(_ => CsharpRunSpec(TestCodeOk));

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