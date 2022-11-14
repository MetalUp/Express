using System.Text.RegularExpressions;
using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class JavaCompiler {
    private static string GetVersion(RunSpec runSpec) {
        var java = $"{CompileServerController.JavaPath}\\bin\\javac.exe";
        var version = Helpers.GetVersion(java, "-version", runSpec);

        return string.IsNullOrEmpty(version) ? "not found" : Regex.Match(version, "javac ([\\d\\.]+)").Groups[1].Value;
    }

    internal static string[] GetNameAndVersion(RunSpec runSpec) => new[] { "java", GetVersion(runSpec) };

    internal static (RunResult, string) Compile(RunSpec runSpec, bool createExecutable) {
        const string tempFileName = "temp.java";
        var file = $"{runSpec.TempDir}{tempFileName}";
        var javaCompiler = $"{CompileServerController.JavaPath}\\bin\\javac.exe";

        File.WriteAllText(file, runSpec.sourcecode);

        return Helpers.Compile(javaCompiler, file, "temp", runSpec);
    }
}