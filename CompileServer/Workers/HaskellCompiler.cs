using System.Text.RegularExpressions;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class HaskellCompiler {
    private static string GetVersion(RunSpec runSpec) {
        var haskell = $"{runSpec.Options.HaskellPath}\\bin\\ghc-9.4.4.exe";
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

    static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        // Get information about the source directory
        var dir = new DirectoryInfo(sourceDir);

        // Check if the source directory exists
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        // Cache directories before we start copying
        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create the destination directory
        Directory.CreateDirectory(destinationDir);

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true);
            }
        }
    }


    internal static (RunResult, string) Compile(RunSpec runSpec) {

        CopyDirectory($"{runSpec.Options.HaskellPath}\\HUnit-1.6.2.0\\src", runSpec.TempDir, true);
        CopyDirectory($"{runSpec.Options.HaskellPath}\\call-stack-0.4.0\\src", runSpec.TempDir, true);

        const string tempFileName = "temp.hs";
        var file = @$"{runSpec.TempDir}{tempFileName}";
        var haskellCompiler = $"{runSpec.Options.HaskellPath}\\bin\\ghc-9.4.4.exe";

        File.WriteAllText(file, runSpec.sourcecode);

        return UpdateLineNumber(Helpers.Compile(haskellCompiler, file, "temp.exe", runSpec));
    }
}