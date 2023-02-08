using CompileServer.Controllers;
using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static CompileServerTest.TestHelpers;

namespace CompileServerTest.Workers;

[TestClass]
public class HaskellCompilerTest {
    private const string SimpleCode =
        @"main = putStrLn ""1""";

    private const string MissingQuote =
        @"main = putStrLn ""1";

    private const string RunTimeFail =
        @"main = do
              head []
              putStrLn ""1""";

    private static string HaskellVersion = "9.4.4";
    private readonly ILogger testLogger = NullLogger.Instance;

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
        const string localDir = @"C:\Haskell944";
        const string appveyorDir =  @"C:\Haskell944";

        if (Directory.Exists(appveyorDir)) {
            CompileServerController.HaskellPath = appveyorDir;
            HaskellVersion = "9.4.4";
        }
        else {
            CompileServerController.HaskellPath = localDir;
        }
    }

    [TestMethod]
    public void TestVersion() {
        var csv = Handler.GetNameAndVersion(HaskellRunSpec(""), testLogger);

        Assert.AreEqual("haskell", csv[0]);
        Assert.AreEqual(HaskellVersion, csv[1]);
    }

    [TestMethod]
    public void TestVersionInParallel() {
        var csvs = Enumerable.Range(1, 10).AsParallel().Select(_ => Handler.GetNameAndVersion(HaskellRunSpec(""), testLogger)).ToArray();

        foreach (var csv in csvs) {
            Assert.AreEqual("haskell", csv[0]);
            Assert.AreEqual(HaskellVersion, csv[1]);
        }
    }

    [TestMethod]
    public void TestCompileOk() {
        using var runSpec = HaskellRunSpec(SimpleCode);
        var rr = Handler.Compile(runSpec, testLogger).Result.Value as RunResult;
        rr.AssertRunResult(Outcome.Ok);
    }

    [TestMethod]
    public void TestCompileAndRunOk() {
        using var runSpec = HaskellRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1\r\n");
    }

    [TestMethod]
    public void TestCompileFailMissingQuote() {
        using var runSpec = HaskellRunSpec(MissingQuote);
        var rr = Handler.Compile(runSpec, testLogger).Result.Value as RunResult;

        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"{runSpec.TempDir}temp.hs:1:19:error:lexicalerrorinstring/characterliteralatendofinput|1|main=putStrLn""1|^");
        Assert.AreEqual(1, rr.line_no);
        Assert.AreEqual(19, rr.col_no);
    }

    [TestMethod]
    public void TestCompileAndRunFail()
    {
        using var runSpec = HaskellRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;

        Assert.IsNotNull(rr);
        rr.stderr = ClearWhiteSpace(rr.stderr);
        rr.AssertRunResult(Outcome.RunTimeError, "", "", @$"temp.exe:Prelude.head:emptylistCallStack(fromHasCallStack):error,calledatlibraries\base\GHC\List.hs:1646:3inbase:GHC.ListerrorEmptyList,calledatlibraries\base\GHC\List.hs:85:11inbase:GHC.ListbadHead,calledatlibraries\base\GHC\List.hs:81:28inbase:GHC.Listhead,calledat{runSpec.TempDir}temp.hs:2:15inmain:Main");
    }

    [TestMethod]
    public void TestCompileAndRunInParallel()
    {
        var runSpecs = Enumerable.Range(1, 10).Select(i => HaskellRunSpec(SimpleCode));

        var rrs = runSpecs.AsParallel().Select(rr => Handler.CompileAndRun(rr, testLogger).Result.Value).Cast<RunResult>().ToArray();

        foreach (var rr in rrs)
        {
            Assert.IsNotNull(rr);
            Assert.AreEqual(Outcome.Ok, rr.outcome);
            Assert.AreEqual("", rr.cmpinfo);
            Assert.AreEqual("1\r\n", rr.stdout);
            Assert.AreEqual("", rr.stderr);
            Assert.AreEqual("", rr.run_id);
        }

        foreach (var testRunSpec in runSpecs)
        {
            testRunSpec.Dispose();
        }
    }

    //[TestMethod]
    //public void TestCompileAndTestInParallel()
    //{
    //    var runSpecs = Enumerable.Range(1, 10).Select(i => HaskellRunSpec(TestCodeOk));

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