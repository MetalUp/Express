using CompileServer.Models;
using CS = Microsoft.CodeAnalysis.CSharp;
using VB = Microsoft.CodeAnalysis.VisualBasic;

namespace CompileServerTest;

public static class TestHelpers {
    private static int LineAdjustment(string l) =>
        l switch {
            "csharp" => 1,
            "python" => 2,
            "java" => 3,
            "haskell" => 1,
            "vb" => 3,
            _ => 0
        };

    private static int ColumnAdjustment(string l) =>
        l switch {
            "csharp" => 0,
            "python" => 0,
            "java" => 0,
            "haskell" => 0,
            "vb" => 0,
            _ => 0
        };

    private static CompileOptions Options(string l) => new() {
        PythonPath = "C:\\Python311",
        JavaPath = GetJavaPath(),
        HaskellPath = @"C:\Haskell944",
        CSharpVersion = CS.LanguageVersion.CSharp10,
        VisualBasicVersion = VB.LanguageVersion.VisualBasic16_9,
        CompileArguments = "--strict --disallow-untyped-defs --show-column-numbers",
        ProcessTimeout = 30000,
        LineAdjustment = LineAdjustment(l),
        ColumnAdjustment = ColumnAdjustment(l)
    };

    public static TestRunSpec CsharpRunSpec(string code) => RunSpec("csharp", code);
    public static TestRunSpec VisualBasicRunSpec(string code) => RunSpec("vb", code);
    public static TestRunSpec JavaRunSpec(string code) => RunSpec("java", code);
    public static TestRunSpec PythonRunSpec(string code) => RunSpec("python", code);

    public static TestRunSpec HaskellRunSpec(string code) => RunSpec("haskell", code);

    public static string ClearWhiteSpace(string s) => s.Replace("\r", "").Replace("\n", "").Replace(" ", "");

    private static TestRunSpec RunSpec(string language, string code) => new() { language_id = language, sourcecode = code, Options = Options(language) };

    private static string GetJavaPath() {
        const string localDir = @"C:\Program Files\Java\jdk-17.0.4.1";
        const string appveyorDir = @"C:\Program Files\Java\jdk17";
        return Directory.Exists(appveyorDir) ? appveyorDir : localDir;
    }

    public static void AssertRunResult(this RunResult rr, Outcome outcome, string cmpinfo = "", string stdout = "", string stderr = "") {
        Assert.AreEqual(outcome, rr.outcome);
        Assert.AreEqual(cmpinfo, rr.cmpinfo);
        Assert.AreEqual(stdout, rr.stdout);
        Assert.AreEqual(stderr, rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    public static void AssertRunResultContains(this RunResult rr, Outcome outcome, string cmpinfo = "", string stdout = "", string stderr = "") {
        Assert.AreEqual(outcome, rr.outcome);
        Assert.AreEqual(cmpinfo, rr.cmpinfo);
        Assert.AreEqual(stdout, rr.stdout);
        Assert.IsTrue(rr.stderr.Contains(stderr), rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    public class TestRunSpec : RunSpec, IDisposable {
        public void Dispose() => base.CleanUp();

        public override void CleanUp() {
            // do nothing
        }
    }
}