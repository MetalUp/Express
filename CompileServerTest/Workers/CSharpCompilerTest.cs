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

    private const string SimpleCodeWithMain = @"
    public class Wrapper {
        static void Main() {
            System.Console.WriteLine(""1"");
        }
    }";

    private const string DivZero = "var a = 1/0;";
    
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

    private readonly ILogger testLogger = NullLogger.Instance;

    [TestMethod]
    public void TestVersion() {
        var csv = CSharpCompiler.GetNameAndVersion();

        Assert.AreEqual("csharp", csv[0]);
        Assert.AreEqual("10", csv[1]);
    }

    [TestMethod]
    public void TestVersionInParallel() {
        var csvs = Enumerable.Range(1, 10).AsParallel().Select(_ => CSharpCompiler.GetNameAndVersion()).ToArray();

        foreach (var csv in csvs) {
            Assert.AreEqual("csharp", csv[0]);
            Assert.AreEqual("10", csv[1]);
        }
    }

    [TestMethod]
    public void TestCompileOk() {
        var runSpec = CsharpRunSpec(SimpleCode);
        var (rr, code) = CSharpCompiler.Compile(runSpec, true);
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok);
        Assert.AreEqual(2048, code.Length);
    }

    [TestMethod]
    public void TestCompileOkWithMain()
    {
        var runSpec = CsharpRunSpec(SimpleCodeWithMain);
        var (rr, code) = CSharpCompiler.Compile(runSpec, true);
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok);
        Assert.AreEqual(2048, code.Length);
    }

    [TestMethod]
    public void TestCompileAndRunOk() {
        var runSpec = CsharpRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1");
    }

    [TestMethod]
    public void TestCompileFailDivisionByZero() {
        var runSpec = CsharpRunSpec(DivZero);
        var (rr, code) = CSharpCompiler.Compile(runSpec, true);
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.CompilationError, "(1,9): error CS0020: Division by constant zero");
        Assert.AreEqual(0, code.Length);
    }

    [TestMethod]
    public void TestCompileAndRunFail() {
        var runSpec = CsharpRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value;

        Assert.IsNotNull(rr);
        rr.AssertRunResultContains(Outcome.RunTimeError, "", "", "Input string was not in a correct format.");
    }

    [TestMethod]
    public void TestCompileAndRunStackOverflow()
    {
        var runSpec = CsharpRunSpec(StackOverFlowFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value;

        Assert.IsNotNull(rr);
        rr.AssertRunResultContains(Outcome.RunTimeError, "", "", "Stack overflow");
    }

    [TestMethod]
    public void TestCompileAndTestOk() {
        var runSpec = CsharpRunSpec(TestCodeOk);
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
        var runSpec = CsharpRunSpec(TestCodeFail);
        var rr = Handler.CompileAndTest(runSpec, testLogger).Result.Value;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Failed!  - Failed:     1, Passed:     0, Skipped:     0, Total:     1"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestRTE()
    {
        var runSpec = CsharpRunSpec(TestCodeRTE);
        var rr = Handler.CompileAndTest(runSpec, testLogger).Result.Value;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Failed!  - Failed:     1, Passed:     0, Skipped:     0, Total:     1"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestStackOverflow()
    {
        var runSpec = CsharpRunSpec(TestCodeStackOverflow);
        var rr = Handler.CompileAndTest(runSpec, testLogger).Result.Value;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.RunTimeError, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stderr.Contains("Stack overflow"));
        //Assert.IsTrue(rr.stdout.Contains("Failed!  - Failed:     1, Passed:     0, Skipped:     0, Total:     1"), rr.stdout);
        Assert.AreEqual("", rr.run_id);
    }

    //[TestMethod]
    //public void TestCompileAndRunInParallel() {
    //    var runSpecs = Enumerable.Range(1, 10).Select(i => CsharpRunSpec(SimpleCode));

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
    //    var runSpecs = Enumerable.Range(1, 10).Select(i => CsharpRunSpec(TestCodeOk));

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