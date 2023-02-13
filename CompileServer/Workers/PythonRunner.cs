using CompileServer.Models;

namespace CompileServer.Workers;

public static class PythonRunner {
    internal static RunResult Execute(string pyFile, RunSpec runSpec, RunResult runResult) {
        var file = $"{runSpec.TempDir}{pyFile}";
        var pythonExe = $"{runSpec.Options.PythonPath}\\python.exe";

        return Helpers.Execute(pythonExe, file, runSpec, runResult);
    }
}