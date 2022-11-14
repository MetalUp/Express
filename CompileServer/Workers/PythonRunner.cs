using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class PythonRunner {
    internal static RunResult Execute(string pyFile, RunResult runResult) {
        var file = $"{runResult.TempDir}{pyFile}";
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";

        return Helpers.Execute(pythonExe, file, runResult);
    }
}