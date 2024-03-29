using System.Globalization;
using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static CompileServerTest.TestHelpers;

namespace CompileServerTest.Workers;

[TestClass]
public class JavaCompilerTest {
    private const string SimpleCode =
        @"public class temp {
        public static void main(String[] args) {
           // StudentCode
           int a = 1;
           System.out.print(a);
        }
        }";

    private const string MissingSC =
        @"public class temp {
        public static void main(String[] args) {
           // StudentCode
           int a = 1 
        }
        }";

    private const string RunTimeFail =
        @"public class temp {
        public static void main(String[] args) {
           // StudentCode
           int a = Integer.parseInt(""invalid"");
           System.out.print(a);
        }
        }";

    private static string JavaVersion = "17.0.4.1";
    private readonly ILogger testLogger = NullLogger.Instance;

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
        const string appveyorDir = @"C:\Program Files\Java\jdk17";

        if (Directory.Exists(appveyorDir)) {
            JavaVersion = "17.0.1";
        }
    }

    [TestInitialize]
    public void Initialize() {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");
    }

    [TestMethod]
    public void TestVersion() {
        var csv = Handler.GetNameAndVersion(JavaRunSpec(""));

        Assert.AreEqual("java", csv[0]);
        Assert.AreEqual(JavaVersion, csv[1]);
    }

    [TestMethod]
    public void TestVersionInParallel() {
        var csvs = Enumerable.Range(1, 10).AsParallel().Select(_ => Handler.GetNameAndVersion(JavaRunSpec(""))).ToArray();

        foreach (var csv in csvs) {
            Assert.AreEqual("java", csv[0]);
            Assert.AreEqual(JavaVersion, csv[1]);
        }
    }

    [TestMethod]
    public void TestCompileOk() {
        using var runSpec = JavaRunSpec(SimpleCode);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok);
    }

    [TestMethod]
    public void TestCompileAndRunOk() {
        using var runSpec = JavaRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1");
    }

    [TestMethod]
    public void TestCompileFailMissingSemiColon() {
        using var runSpec = JavaRunSpec(MissingSC);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"{runSpec.TempDir}temp.java:4:error:';'expectedinta=1^1error");
        Assert.AreEqual(1, rr.line_no);
        Assert.AreEqual(21, rr.col_no);
    }

    [TestMethod]
    public void TestCompileAndRunFail() {
        using var runSpec = JavaRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;

        Assert.IsNotNull(rr);
        rr.stderr = ClearWhiteSpace(rr.stderr);
        rr.AssertRunResult(Outcome.RunTimeError, "", "", @"Exceptioninthread""main""java.lang.NumberFormatException:Forinputstring:""invalid""	atjava.base/java.lang.NumberFormatException.forInputString(NumberFormatException.java:67)	atjava.base/java.lang.Integer.parseInt(Integer.java:668)	atjava.base/java.lang.Integer.parseInt(Integer.java:786)	attemp.main(temp.java:4)");
    }

    [TestMethod]
    public void TestCompileAndRunInParallel() {
        var runSpecs = Enumerable.Range(1, 10).Select(_ => JavaRunSpec(SimpleCode));

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

    //[TestMethod]
    //public void TestCompileAndTestInParallel()
    //{
    //    var runSpecs = Enumerable.Range(1, 10).Select(i => JavaRunSpec(TestCodeOk));

    //    var rrs = runSpecs.AsParallel().Select(rr => Handler.CompileAndTest(rr, testLogger).Result.Value).Cast<RunResult>().ToArray();

    //    foreach (var rr in rrs)
    //    {
    //        Assert.IsNotNull(rr);
    //        Assert.AreEqual(Outcome.Ok, rr.outcome);
    //        Assert.AreEqual("", rr.cmpinfo);
    //        Assert.IsTrue(rr.stdout.Contains("Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1"), rr.stdout);
    //        Assert.AreEqual("", rr.stderr);
    //        Assert.AreEqual("", rr.run_id);
    //    }

    //    foreach (var testRunSpec in runSpecs)
    //    {
    //        testRunSpec.Dispose();
    //    }
    //}
}