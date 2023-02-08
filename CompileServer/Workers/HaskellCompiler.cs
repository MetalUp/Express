using System.Text.RegularExpressions;
using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class HaskellCompiler {
    private static string GetVersion(RunSpec runSpec) {
        var haskell = $"{CompileServerController.HaskellPath}\\bin\\ghc-9.4.4.exe";
        var version = Helpers.GetVersion(haskell, "--version", runSpec);

        return string.IsNullOrEmpty(version) ? "not found" : Regex.Match(version, "version ([\\d\\.]+)").Groups[1].Value;
    }

    internal static string[] GetNameAndVersion(RunSpec runSpec) => new[] { "haskell", GetVersion(runSpec) };

    private static (RunResult, string) UpdateLineNumber((RunResult, string) result) {
        var (rr, _) = result;
        if (rr.outcome == Outcome.CompilationError) {
            try {
                var err = rr.cmpinfo.Split(":");
                if (int.TryParse(err[2], out var lineNo)) {
                    rr.line_no = lineNo;
                }
                if (int.TryParse(err[3], out var colNo)) {
                    rr.col_no = colNo;
                }
            }
            catch (Exception) {
                // ignore all exceptions
            }
        }

        return result;
    }


    internal static (RunResult, string) Compile(RunSpec runSpec, bool createExecutable) {
        const string tempFileName = "temp.hs";
        var file = $"{runSpec.TempDir}{tempFileName}";
        var haskellCompiler = $"{CompileServerController.HaskellPath}\\bin\\ghc-9.4.4.exe";

        File.WriteAllText(file, runSpec.sourcecode);

        return  UpdateLineNumber(Helpers.Compile(haskellCompiler, file, "temp.exe", runSpec));
    }
}