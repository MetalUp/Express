using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class PythonCompiler {
    private static string GetVersion(RunSpec runSpec) {
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";
        var version = Helpers.GetVersion(pythonExe, "--version", runSpec);

        return string.IsNullOrEmpty(version) ? "not found" : version.Replace("Python ", "").Trim();
    }

    internal static string[] GetNameAndVersion(RunSpec runSpec) => new[] { "python", GetVersion(runSpec) };

    internal static (RunResult, string) Compile(RunSpec runSpec, bool createExecutable) {
        const string tempFileName = "temp.py";
        var file = $"{runSpec.TempDir}{tempFileName}";
        File.WriteAllText(file, runSpec.sourcecode);
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";
        var args = $"-m py_compile {file}";

        return Helpers.Compile(pythonExe, args, createExecutable ? tempFileName : "", runSpec);
    }
}