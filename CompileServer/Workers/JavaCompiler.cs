using System.Text.RegularExpressions;
using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class JavaCompiler {
    private static string GetVersion() {
        var java = $"{CompileServerController.JavaPath}\\bin\\javac.exe";
        var version = Helpers.GetVersion(java, "-version");

        return string.IsNullOrEmpty(version) ? "not found" : Regex.Match(version, "javac ([\\d\\.]+)").Groups[1].Value;
    }

    public static string[] GetNameAndVersion() => new[] { "java", GetVersion() };

    public static (RunResult, string) Compile(RunSpec runSpec) {
        const string tempFileName = "temp.java";
        var file = $"{(string?)Path.GetTempPath()}{tempFileName}";
        var javaCompiler = $"{CompileServerController.JavaPath}\\bin\\javac.exe";

        File.WriteAllText(file, runSpec.sourcecode);

        return Helpers.Compile(javaCompiler, file, file, "temp");
    }
}