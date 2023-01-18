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

    private static (RunResult, string) UpdateLineNumber((RunResult, string) result) {
        var (rr, s) = result;
        if (rr.outcome == Outcome.CompilationError) {
            try {
                var err = rr.cmpinfo.Split("\n");
                var lineErr = err[0].Split(":")[2];
                if (int.TryParse(lineErr, out var lineNo)) {
                    rr.line_no = lineNo;
                }

                var colErr = err[2];
                rr.col_no = colErr[..(colErr.IndexOf('^') + 1)].Length;
            }
            catch (Exception _) {
                // ignore all exceptions
            }
        }

        return result;
    }


    internal static (RunResult, string) Compile(RunSpec runSpec, bool createExecutable) {
        const string tempFileName = "temp.java";
        var file = $"{runSpec.TempDir}{tempFileName}";
        var javaCompiler = $"{CompileServerController.JavaPath}\\bin\\javac.exe";

        File.WriteAllText(file, runSpec.sourcecode);

        return  UpdateLineNumber(Helpers.Compile(javaCompiler, file, "temp", runSpec));
    }
}