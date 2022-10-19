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

    public static string[] GetNameAndVersion() => new[] { "java", GetVersion() };

    public static (RunResult, string) Compile(RunSpec runSpec) {
        const string tempFileName = "temp.java";
        var file = $"{(string?)Path.GetTempPath()}{tempFileName}";
        var javaCompiler = $"{CompileServerController.JavaPath}\\bin\\javac.exe";

        File.WriteAllText(file, runSpec.sourcecode);

        var runResult = new RunResult();

        try {
            using var process = Helpers.CreateProcess(javaCompiler, file);
            process.WaitForExit();
            runResult = Helpers.SetCompileResults(process, runResult);
        }
        catch (Exception e) {
            runResult = Helpers.SetCompileResults(runResult, e);
        }
        finally {
            File.Delete(file);
        }

        return (runResult, "temp");
    }
}