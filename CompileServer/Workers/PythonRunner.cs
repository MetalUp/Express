using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class PythonRunner {
    public static RunResult Execute(string pyFile) {
        var file = $"{Path.GetTempPath()}{pyFile}";
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";

        return Helpers.Execute(pythonExe, file, file);
    }
}