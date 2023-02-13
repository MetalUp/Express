using CompileServer.Models;

namespace CompileServer.Workers;

public static class PythonTester {
    internal static RunResult Execute(string pyFile, RunSpec runSpec, RunResult runResult) {
        var file = $"{runSpec.TempDir}{pyFile}";
        var pythonExe = $"{runSpec.Options.PythonPath}\\python.exe";
        var args = $"-m unittest {file}";

        var rr = Helpers.Execute(pythonExe, args, runSpec, runResult);

        // since python tester writes by default to stderr swap to stdout
        rr.stdout = rr.stderr;
        rr.stderr = "";
        rr.outcome = Outcome.Ok;
        return rr;
    }
}