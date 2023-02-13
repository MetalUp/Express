using CompileServer.Models;

namespace CompileServer.Workers;

public static class HaskellTester {
    internal static RunResult Execute(string pyFile, RunResult runResult) =>
        //var file = $"{runResult.TempDir}{pyFile}";
        //var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";
        //var args = $"-m unittest {file}";
        //var rr = Helpers.Execute(pythonExe, args, runResult);
        //// since python tester writes by default to stderr swap to stdout
        //rr.stdout = rr.stderr;
        //rr.stderr = "";
        //rr.outcome = Outcome.Ok;
        //return rr;
        new();
}