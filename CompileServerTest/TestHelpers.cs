using CompileServer.Models;

namespace CompileServerTest;

public static class TestHelpers {
    public static TestRunSpec CsharpRunSpec(string code) => RunSpec("csharp", code);
    public static TestRunSpec VisualBasicRunSpec(string code) => RunSpec("vb", code);
    public static TestRunSpec JavaRunSpec(string code) => RunSpec("java", code);
    public static TestRunSpec PythonRunSpec(string code) => RunSpec("python", code);

    public static string ClearWhiteSpace(string s) => s.Replace("\r", "").Replace("\n", "").Replace(" ", "");

    private static TestRunSpec RunSpec(string language, string code) => new() { language_id = language, sourcecode = code };

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
        public override void CleanUp() {
            // do nothing
        }

        public void Dispose() => base.CleanUp();
    }
}