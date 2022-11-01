using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class PythonTester {
    public static RunResult Execute(string pyFile) {
        var file = $"{Path.GetTempPath()}{pyFile}";
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";
        var args = $"-m unittest {file}";

        var rr = Helpers.Execute(pythonExe, args, file);

        // since python tester writes by default to stderr swap to stdout
        rr.stdout = rr.stderr;
        rr.stderr = "";
        rr.outcome = Outcome.Ok;
        return rr;
    }
}