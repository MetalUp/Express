using CompileServer.Models;

namespace CompileServer.Workers;

public static class HaskellRunner {
    internal static RunResult Execute(string file, RunSpec runSpec, RunResult runResult) {
        var exe = $"{runSpec.TempDir}{file}";
        return Helpers.Execute(exe, "", runSpec, runResult);
    }
}