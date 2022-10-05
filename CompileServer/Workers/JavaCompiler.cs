using System.Diagnostics;
using System.Text.RegularExpressions;
using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class JavaCompiler {
    public static string GetVersion() {
        var java = $"{CompileServerController.JavaPath}\\bin\\javac.exe";
        string version;

        try {
            using var process = Helpers.CreateProcess(java, "-version");
            process.WaitForExit();
            using var stdOutput = process.StandardOutput;
            version = stdOutput.ReadToEnd();
        }
        catch (Exception e) {
            version = e.Message;
        }

        return string.IsNullOrEmpty(version) ? "not found" : Regex.Match(version, "javac ([\\d\\.]+)").Groups[1].Value;
    }

    public static (RunResult, string) Compile(RunSpec runSpec) {
        const string tempFileName = "temp.java";
        var file = $"{(string?)Path.GetTempPath()}{tempFileName}";
        var javaCompiler = $"{CompileServerController.JavaPath}\\bin\\javac.exe";

        File.WriteAllText(file, runSpec.sourcecode);

        var runResult = new RunResult();

        try {
            using var process = Helpers.CreateProcess(javaCompiler, file);

            process.WaitForExit();

            using var stdErr = process.StandardError;
            runResult.cmpinfo = stdErr.ReadToEnd();
            runResult.outcome = string.IsNullOrEmpty(runResult.cmpinfo) ? Outcome.Ok : Outcome.CompilationError;
        }
        catch (Exception e) {
            runResult.outcome = Outcome.CompilationError;
            runResult.stderr = e.Message;
        }
        finally {
            File.Delete(file);
        }

        return (runResult, "temp");
    }
}