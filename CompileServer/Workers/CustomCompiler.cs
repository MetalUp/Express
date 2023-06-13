using CompileServer.Models;

namespace CompileServer.Workers;

public static class CustomCompiler {
    private const string CustomExeName = "bc.exe";
    private const string TempFileName = "temp.bcn";
    private const string TempCsFileName = "temp.cs";

    private static string GetCustomExe(RunSpec runSpec) => $"D:\\home\bacon\\{CustomExeName}";

    private static string GetVersion(RunSpec runSpec) {
        var version = Helpers.GetVersion(GetCustomExe(runSpec), "--version", runSpec);
        return string.IsNullOrEmpty(version) ? "not found" : version.Trim();
    }

    internal static string[] GetNameAndVersion(RunSpec runSpec) => new[] { "bacon", GetVersion(runSpec) };

    private static string CreateSourceFile(RunSpec runSpec) {
        var file = $"{runSpec.TempDir}{TempFileName}";
        File.WriteAllText(file, runSpec.sourcecode);
        return file;
    }

    private static (RunResult, string) Compile(RunSpec runSpec, string file, string tempFileName) =>
        Helpers.Compile(GetCustomExe(runSpec), $"{file}", tempFileName, runSpec);

    internal static (RunResult,string) Compile(RunSpec runSpec) => Compile(runSpec, CreateSourceFile(runSpec), TempCsFileName);
}