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
        @"
print (str(1/))";

    private const string MissingTermMultiLine =
        @"
print (str(1))
print (str(2))
print (str(1/))";

    private const string RunTimeFail =
        @"print (int(""invalid""))";

    private const string TestCodeOk =
        @"import unittest

class Tests(unittest.TestCase):

    def test_Ok(self) -> None:
        assert sum([1, 2, 3]) == 6, ""Should be 6""

if __name__ == ""__main__"":
    unittest.main()
    print(""Everything passed"")
";

    private const string TestCodeFail =
        @"import unittest

class Tests(unittest.TestCase):

    def test_Ok(self) -> None:
        assert sum([1, 2, 4]) == 6, ""Should be 6""

if __name__ == ""__main__"":
    unittest.main()
    print(""Everything passed"")
";

    private const string TestCodeRTE =
        @"import unittest

class Tests(unittest.TestCase):

    def test_Ok(self) -> None:
        print (int(""invalid""))

if __name__ == ""__main__"":
    unittest.main()
    print(""Everything passed"")
";

    private const string TestCodeTypeFail = "def N(c): return c - 20 if c > 19 else c +  380";

    private readonly ILogger testLogger = NullLogger.Instance;

    [ClassInitialize]
    public static void Initialize(TestContext testContext) { }

    [TestInitialize]
    public void StartTest() { }

    [TestMethod]
    public void TestVersion() {
        var csv = Handler.GetNameAndVersion(PythonRunSpec(""));

        Assert.AreEqual("python", csv[0]);
        Assert.IsTrue(csv[1].StartsWith("3.11."));
    }

    [TestMethod]
    public void TestVersionInParallel() {
        var csvs = Enumerable.Range(1, 10).AsParallel().Select(_ => Handler.GetNameAndVersion(PythonRunSpec(""))).ToArray();

        foreach (var csv in csvs) {
            Assert.AreEqual("python", csv[0]);
            Assert.IsTrue(csv[1].StartsWith("3.11."));
        }
    }

    [TestMethod]
    public void TestTypeCheckOk() {
        using var runSpec = PythonRunSpec(SimpleCode);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok);
    }

    [TestMethod]
    public void TestCompileOk() {
        using var runSpec = PythonRunSpec(SimpleCode);
        runSpec.Options.PythonUseTypeAnnotations = false;
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok);
    }

    [TestMethod]
    public void TestTypeCheckAndRunOk() {
        using var runSpec = PythonRunSpec(SimpleCode);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1\r\n");
    }

    [TestMethod]
    public void TestCompileAndRunOk() {
        using var runSpec = PythonRunSpec(SimpleCode);
        runSpec.Options.PythonUseTypeAnnotations = false;
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.AssertRunResult(Outcome.Ok, "", "1\r\n");
    }

    [TestMethod]
    public void TestTypeCheckFail() {
        using var runSpec = PythonRunSpec(TestCodeTypeFail);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @"temp.py:1:1:error:Functionismissingatypeannotation[no-untyped-def]Found1errorin1file(checked1sourcefile)");
        Assert.AreEqual(1, rr.line_no);
        Assert.AreEqual(1, rr.col_no);
    }

    [TestMethod]
    public void TestTypeCheckFailMissingTerm() {
        using var runSpec = PythonRunSpec(MissingTerm);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @"temp.py:2:15:error:invalidsyntax[syntax]Found1errorin1file(errorspreventedfurtherchecking)");
        Assert.AreEqual(1, rr.line_no);
        Assert.AreEqual(15, rr.col_no);
    }

    [TestMethod]
    public void TestCompileFailMissingTerm() {
        using var runSpec = PythonRunSpec(MissingTerm);
        runSpec.Options.PythonUseTypeAnnotations = false;
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"File""{runSpec.TempDir}temp.py"",line2print(str(1/))^SyntaxError:invalidsyntax");
        Assert.AreEqual(2, rr.line_no);
        Assert.AreEqual(18, rr.col_no);
    }

    [TestMethod]
    public void TestTypeCheckFailMissingTermMultiLine() {
        using var runSpec = PythonRunSpec(MissingTermMultiLine);
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @"temp.py:4:15:error:invalidsyntax[syntax]Found1errorin1file(errorspreventedfurtherchecking)");
        Assert.AreEqual(3, rr.line_no);
        Assert.AreEqual(15, rr.col_no);
    }

    [TestMethod]
    public void TestCompileFailMissingTermMultiLine() {
        using var runSpec = PythonRunSpec(MissingTermMultiLine);
        runSpec.Options.PythonUseTypeAnnotations = false;
        var rr = Handler.Compile(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        rr.cmpinfo = ClearWhiteSpace(rr.cmpinfo);

        rr.AssertRunResult(Outcome.CompilationError, @$"File""{runSpec.TempDir}temp.py"",line4print(str(1/))^SyntaxError:invalidsyntax");
        Assert.AreEqual(4, rr.line_no);
        Assert.AreEqual(18, rr.col_no);
    }

    [TestMethod]
    public void TestTypeCheckAndRunFail() {
        using var runSpec = PythonRunSpec(RunTimeFail);
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;

        Assert.IsNotNull(rr);
        rr.stderr = ClearWhiteSpace(rr.stderr);

        rr.AssertRunResult(Outcome.RunTimeError, "", "", @$"Traceback(mostrecentcalllast):File""{runSpec.TempDir}temp.py"",line1,in<module>print(int(""invalid""))^^^^^^^^^^^^^^ValueError:invalidliteralforint()withbase10:'invalid'");
    }

    [TestMethod]
    public void TestCompileAndRunFail() {
        using var runSpec = PythonRunSpec(RunTimeFail);
        runSpec.Options.PythonUseTypeAnnotations = false;
        var rr = Handler.CompileAndRun(runSpec, testLogger).Result.Value as RunResult;

        Assert.IsNotNull(rr);
        rr.stderr = ClearWhiteSpace(rr.stderr);

        rr.AssertRunResult(Outcome.RunTimeError, "", "", @$"Traceback(mostrecentcalllast):File""{runSpec.TempDir}temp.py"",line1,in<module>print(int(""invalid""))^^^^^^^^^^^^^^ValueError:invalidliteralforint()withbase10:'invalid'");
    }

    [TestMethod]
    public void TestTypeCheckAndTestOk() {
        using var runSpec = PythonRunSpec(TestCodeOk);
        var rr = Handler.CompileAndTest(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Ran 1 test in"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("OK"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestOk() {
        using var runSpec = PythonRunSpec(TestCodeOk);
        runSpec.Options.PythonUseTypeAnnotations = false;
        var rr = Handler.CompileAndTest(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Ran 1 test in"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("OK"), rr.stdout);
        Assert.AreEqual("", rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestTypeCheckAndTestFail() {
        using var runSpec = PythonRunSpec(TestCodeFail);
        var rr = Handler.CompileAndTest(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Ran 1 test in"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("Should be 6"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("FAIL"), rr.stdout);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestFail() {
        using var runSpec = PythonRunSpec(TestCodeFail);
        runSpec.Options.PythonUseTypeAnnotations = false;
        var rr = Handler.CompileAndTest(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Ran 1 test in"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("Should be 6"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("FAIL"), rr.stdout);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestTypeCheckAndTestRTE() {
        using var runSpec = PythonRunSpec(TestCodeRTE);
        var rr = Handler.CompileAndTest(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Ran 1 test in"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("invalid literal"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("ERROR"), rr.stdout);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestCompileAndTestRTE() {
        using var runSpec = PythonRunSpec(TestCodeRTE);
        runSpec.Options.PythonUseTypeAnnotations = false;
        var rr = Handler.CompileAndTest(runSpec).Result.Value as RunResult;
        Assert.IsNotNull(rr);
        Assert.AreEqual(Outcome.Ok, rr.outcome);
        Assert.AreEqual("", rr.cmpinfo);
        Assert.IsTrue(rr.stdout.Contains("Ran 1 test in"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("invalid literal"), rr.stdout);
        Assert.IsTrue(rr.stdout.Contains("ERROR"), rr.stdout);
        Assert.AreEqual("", rr.run_id);
    }

    [TestMethod]
    public void TestTypeCheckAndRunInParallel() {
        var runSpecs = Enumerable.Range(1, 10).Select(_ => PythonRunSpec(SimpleCode));

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
    public void TestCompileAndRunInParallel() {
        var runSpecs = Enumerable.Range(1, 10).Select(_ => PythonRunSpec(SimpleCode));
        foreach (var testRunSpec in runSpecs) {
            testRunSpec.Options.PythonUseTypeAnnotations = false;
        }

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
    public void TestTypeCheckAndTestInParallel() {
        var runSpecs = Enumerable.Range(1, 10).Select(_ => PythonRunSpec(TestCodeOk));

        var rrs = runSpecs.AsParallel().Select(rr => Handler.CompileAndTest(rr).Result.Value).Cast<RunResult>().ToArray();

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

    [TestMethod]
    public void TestCompileAndTestInParallel() {
        var runSpecs = Enumerable.Range(1, 10).Select(_ => PythonRunSpec(TestCodeOk));
        foreach (var testRunSpec in runSpecs) {
            testRunSpec.Options.PythonUseTypeAnnotations = false;
        }

        var rrs = runSpecs.AsParallel().Select(rr => Handler.CompileAndTest(rr).Result.Value).Cast<RunResult>().ToArray();

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