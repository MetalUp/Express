using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class PythonCompiler {
    public static string GetVersion() {
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";
        string version;

        try {
            using var process = Helpers.CreateProcess(pythonExe, "--version");
            process.WaitForExit();
            using var stdOutput = process.StandardOutput;
            version = stdOutput.ReadToEnd();
        }
        catch (Exception e) {
            version = e.Message;
        }

        return string.IsNullOrEmpty(version) ? "not found" : version.Replace("Python ", "").Trim();
    }

    public static (RunResult, string) Compile(RunSpec runSpec) {
        const string tempFileName = "temp.py";
        var file = $"{Path.GetTempPath()}{tempFileName}";
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";

        File.WriteAllText(file, runSpec.sourcecode);

        var runResult = new RunResult();
        var args = $"-m py_compile {file}";

        try {
            using var process = Helpers.CreateProcess(pythonExe, args);
            process.WaitForExit();
            runResult = Helpers.SetCompileResults(process, runResult);
        }
        catch (Exception e) {
            runResult = Helpers.SetCompileResults(runResult, e);
        }

        return (runResult, tempFileName);
    }
}