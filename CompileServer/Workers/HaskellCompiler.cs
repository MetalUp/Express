﻿using System.Text.RegularExpressions;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class HaskellCompiler {
    private static string GetVersion(RunSpec runSpec) {
        var haskell = $"{runSpec.Options.HaskellPath}\\bin\\ghc-9.4.4.exe";
        var version = Helpers.GetVersion(haskell, "--version", runSpec);

        return string.IsNullOrEmpty(version) ? "not found" : Regex.Match(version, "version ([\\d\\.]+)").Groups[1].Value;
    }

    internal static string[] GetNameAndVersion(RunSpec runSpec) => new[] { "haskell", GetVersion(runSpec) };

    private static (RunResult, string) UpdateLineNumber((RunResult, string) result, RunSpec runSpec) {
        var (rr, _) = result;
        if (rr.outcome == Outcome.CompilationError) {
            try {
                var err = rr.cmpinfo.Split(":");
                if (int.TryParse(err[2], out var lineNo)) {
                    rr.line_no = Helpers.AdJustCompilerOffset(lineNo, runSpec.Options.LineAdjustment);
                }

                if (int.TryParse(err[3], out var colNo)) {
                    rr.col_no = Helpers.AdJustCompilerOffset(colNo, runSpec.Options.ColumnAdjustment);
                }
            }
            catch (Exception) {
                // ignore all exceptions
            }
        }

        return result;
    }

    private static void CopyDirectory(string sourceDir, string destinationDir) {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Cache directories before we start copying
        var dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (var file in dir.GetFiles()) {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        foreach (var subDir in dirs) {
            var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir);
        }
    }

    internal static (RunResult, string) Compile(RunSpec runSpec) {
        CopyDirectory($"{runSpec.Options.HaskellPath}\\HUnit-1.6.2.0\\src", runSpec.TempDir);
        CopyDirectory($"{runSpec.Options.HaskellPath}\\call-stack-0.4.0\\src", runSpec.TempDir);

        const string tempFileName = "temp.hs";
        var file = @$"{runSpec.TempDir}{tempFileName}";
        var haskellCompiler = $"{runSpec.Options.HaskellPath}\\bin\\ghc-9.4.4.exe";

        File.WriteAllText(file, runSpec.sourcecode);

        return UpdateLineNumber(Helpers.Compile(haskellCompiler, file, "temp.exe", runSpec), runSpec);
    }
}