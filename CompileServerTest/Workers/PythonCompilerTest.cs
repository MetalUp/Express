using CompileServer.Controllers;
using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static CompileServerTest.TestHelpers;

namespace CompileServerTest.Workers;

[TestClass]
public class PythonCompilerTest {
    private const string SimpleCode =
        @"print (str(1))";

    private const string MissingTerm =
        @"print (str(1/))";

    private const string MissingTermMultiLine =
        @"print (str(1))
print (str(2))
print (str(1/))";

    private const string RunTimeFail =
        @"print (int(""invalid""))";

    private const string TestCodeOk =
        @"import unittest

class Tests(unittest.TestCase):

    def test_Ok(self):
        assert sum([1, 2, 3]) == 6, ""Should be 6""

if __name__ == ""__main__"":
    unittest.main()
    print(""Everything passed"")
";

    private const string TestCodeFail =
        @"import unittest

class Tests(unittest.TestCase):

    def test_Ok(self):
        assert sum([1, 2, 4]) == 6, ""Should be 6""

if __name__ == ""__main__"":
    unittest.main()
    print(""Everything passed"")
";

    private const string TestCodeRTE =
        @"import unittest

class Tests(unittest.TestCase):

    def test_Ok(self):
        print (int(""invalid""))

if __name__ == ""__main__"":
    unittest.main()
    print(""Everything passed"")
";

    private readonly ILogger testLogger = NullLogger.Instance;

    [ClassInitialize]
    public static void Initialize(TestContext testContext) {
        CompileServerController.PythonPath = "C:\\Users\\scasc\\AppData\\Local\\Programs\\Python\\Python311";
    }

    [TestMethod]
    public void TestVersion() {
        var csv = Handler.GetNameAndVersion(PythonRunSpec(""), testLogger);

        Assert.AreEqual("python", csv[0]);
        Assert.IsTrue(csv[1].StartsWith("3.11."));
    }

    [TestMethod]
    public void TestVersionInParallel() {
        var csvs = Enumerable.Range(1, 10).AsParallel().Select(_ => Handler.GetNameAndVersion(PythonRunSpec(""), testLogger)).ToArray();

        foreach (var csv in csvs) {
            Assert.AreEqual("python", csv[0]);
            Assert.IsTrue(csv[1].StartsWith("3.11."));
        }
    }

    [TestMethod]
    public void TestCompileOk() {
        using var runSpec = PythonRunSpec(SimpleCode);
        var rr = Handler.Compile(runSpec, testLogger).Result.Value as RunResult;
        rr.AssertRunResult(Outcome.Ok);
    }

    [TestMethod]
    public void TestCompileAndRunOk() {
        using var runSpec = PythonRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1\r\n");
    }

    [TestMethod]
    public void TestCompileFailMissingTerm() {
        using var runSpec = PythonRunSpec(MissingTerm);
        var rr = Handler.Compile(runSpec, testLogger).Result.Value as RunResult;

        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"File""{runSpec.TempDir}temp.py"",line1print(str(1/))^SyntaxError:invalidsyntax");
        Assert.AreEqual(1, rr.line_no);
        Assert.AreEqual(18, rr.col_no);
    }

    [TestMethod]
    public void TestCompileFailMissingTermMultiLine() {
        using var runSpec = PythonRunSpec(MissingTermMultiLine);
        var rr = Handler.Compile(runSpec, testLogger).Result.Value as RunResult;

        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"File""{runSpec.TempDir}temp.py"",line3print(str(1/))^SyntaxError:invalidsyntax");
        Assert.AreEqual(3, rr.line_no);
        Assert.AreEqual(18, rr.col_no);
    }

    [TestMethod]
    public void TestCompileAndRunFail() {
        using var runSpec = PythonRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;

        Assert.IsNotNull(rr);
        rr.stderr = ClearWhiteSpace(rr.stderr);

        rr.AssertRunResult(Outcome.RunTimeError, "", "", @$"Traceback(mostrecentcalllast):File""{runSpec.TempDir}temp.py"",line1,in<module>print(int(""invalid""))^^^^^^^^^^^^^^ValueError:invalidliteralforint()withbase10:'invalid'");
    }

    [TestMethod]
    public void TestCompileAndTestOk() {
        using var runSpec = PythonRunSpec(TestCodeOk);
        var rr = Handler.CompileAndTest(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Ran 1 test in"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("OK"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestFail() {
        using var runSpec = PythonRunSpec(TestCodeFail);
        var rr = Handler.CompileAndTest(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Ran 1 test in"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("Should be 6"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("FAIL"), rr.stdout);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestRTE() {
        using var runSpec = PythonRunSpec(TestCodeRTE);
        var rr = Handler.CompileAndTest(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Ran 1 test in"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("invalid literal"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("ERROR"), rr.stdout);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndRunInParallel() {
        var runSpecs = Enumerable.Range(1, 10).Select(i => PythonRunSpec(SimpleCode));

        var rrs = runSpecs.AsParallel().Select(rr => Handler.CompileAndRun(rr, testLogger).Result.Value).Cast<RunResult>().ToArray();

        foreach (var rr in rrs) {
            Assert.IsNotNull(rr);
            rr.AssertRunResult(Outcome.Ok, "", "1\r\n");
        }

        foreach (var testRunSpec in runSpecs) {
            testRunSpec.Dispose();
        }
    }

    [TestMethod]
    public void TestCompileAndTestInParallel() {
        var runSpecs = Enumerable.Range(1, 10).Select(i => PythonRunSpec(TestCodeOk));

        var rrs = runSpecs.AsParallel().Select(rr => Handler.CompileAndTest(rr, testLogger).Result.Value).Cast<RunResult>().ToArray();

        foreach (var rr in rrs) {
            Assert.IsNotNull(rr);
            Assert.AreEqual(Outcome.Ok, rr.outcome);
            Assert.AreEqual("", rr.cmpinfo);
            Assert.IsTrue(rr.stdout.Contains("Ran 1 test in"), rr.stdout);
            Assert.IsTrue(rr.stdout.Contains("OK"), rr.stdout);
            Assert.AreEqual("", rr.stderr);
            Assert.AreEqual("", rr.run_id);
        }

        foreach (var testRunSpec in runSpecs) {
            testRunSpec.Dispose();
        }
    }
}