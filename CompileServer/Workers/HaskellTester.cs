using CompileServer.Models;

namespace CompileServer.Workers;

public static class HaskellTester {
    internal static RunResult Execute(string file, RunSpec runSpec, RunResult runResult) {
        var exe = $"{runSpec.TempDir}{file}";
        var rr = Helpers.Execute(exe, "", runSpec, runResult);

        // since haskell tester writes by default to stderr swap to stdout and take last line;

        var result = rr.stderr.Trim().Split('\n').Last();
        rr.stdout = result;
        rr.stderr = "";
        rr.outcome = Outcome.Ok;

        return rr;
    }
}