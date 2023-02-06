using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class HaskellRunner {
    internal static RunResult Execute(string file, RunResult runResult) {
        var exe = $"{runResult.TempDir}{file}";
        return Helpers.Execute(exe, "", runResult);
    }
}