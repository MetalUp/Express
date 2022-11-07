using CompileServer.Models;

namespace CompileServerTest;

public static class TestHelpers {
    public static RunSpec CsharpRunSpec(string code) => RunSpec("csharp", code);
    public static RunSpec VisualBasicRunSpec(string code) => RunSpec("vb", code);
    public static RunSpec JavaRunSpec(string code) => RunSpec("java", code);
    public static RunSpec PythonRunSpec(string code) => RunSpec("python", code);

    public static string ClearWhiteSpace(string s) => s.Replace("\r", "").Replace("\n", "").Replace(" ", "");

   


    public static RunSpec RunSpec(string language, string code) => new() { language_id = language, sourcecode = code };

    public static void AssertRunResult(this RunResult rr, Outcome outcome, string cmpinfo = "", string stdout = "", string stderr = "") {
        Assert.AreEqual(outcome, rr.outcome);
        Assert.AreEqual(cmpinfo, rr.cmpinfo);
        Assert.AreEqual(stdout, rr.stdout);
        Assert.AreEqual(stderr, rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }

    public static void AssertRunResultContains(this RunResult rr, Outcome outcome, string cmpinfo = "", string stdout = "", string stderr = "")
    {
        Assert.AreEqual(outcome, rr.outcome);
        Assert.AreEqual(cmpinfo, rr.cmpinfo);
        Assert.AreEqual(stdout, rr.stdout);
        Assert.IsTrue(rr.stderr.Contains(stderr), rr.stderr);
        Assert.AreEqual("", rr.run_id);
    }
}