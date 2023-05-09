using System.Text.RegularExpressions;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class JavaCompiler {
    private static string GetVersion(RunSpec runSpec) {
        var java = $"{runSpec.Options.JavaPath}\\bin\\javac.exe";
        var version = Helpers.GetVersion(java, "-version", runSpec);

        return string.IsNullOrEmpty(version) ? "not found" : Regex.Match(version, "javac ([\\d\\.]+)").Groups[1].Value;
    }

    internal static string[] GetNameAndVersion(RunSpec runSpec) => new[] { "java", GetVersion(runSpec) };

    private static (RunResult, string) UpdateLineNumber((RunResult, string) result, RunSpec runSpec) {
        var (rr, _) = result;
        if (rr.outcome == Outcome.CompilationError) {
            try {
                var err = rr.cmpinfo.Split("\n");
                var lineErr = err[0].Split(":")[2];
                if (int.TryParse(lineErr, out var lineNo)) {
                    rr.line_no = Helpers.AdJustCompilerOffset(lineNo, runSpec.Options.LineAdjustment);
                }

                var colErr = err[2];
                var colNo = colErr[..(colErr.IndexOf('^') + 1)].Length;
                rr.col_no = Helpers.AdJustCompilerOffset(colNo, runSpec.Options.ColumnAdjustment);
            }
            catch (Exception) {
                // ignore all exceptions
            }
        }

        return result;
    }

    internal static (RunResult, string) Compile(RunSpec runSpec) {
        const string tempFileName = "temp.java";
        var file = $"{runSpec.TempDir}{tempFileName}";
        var javaCompiler = $"{runSpec.Options.JavaPath}\\bin\\javac.exe";

        File.WriteAllText(file, runSpec.sourcecode);

        return UpdateLineNumber(Helpers.Compile(javaCompiler, file, "temp", runSpec), runSpec);
    }
}