using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class PythonCompiler {
    private static string GetVersion() {
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";
        var version = Helpers.GetVersion(pythonExe, "--version");

        return string.IsNullOrEmpty(version) ? "not found" : version.Replace("Python ", "").Trim();
    }

    public static string[] GetNameAndVersion() => new[] { "python", GetVersion() };

    public static (RunResult, string) Compile(RunSpec runSpec) {
        const string tempFileName = "temp.py";
        var file = $"{Path.GetTempPath()}{tempFileName}";
        File.WriteAllText(file, runSpec.sourcecode);
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";
        var args = $"-m py_compile {file}";

        return Helpers.Compile(pythonExe, args, "", tempFileName);
    }
}