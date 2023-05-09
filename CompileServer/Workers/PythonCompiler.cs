using CompileServer.Models;

namespace CompileServer.Workers;

public static class PythonCompiler {
    private const string PythonExeName = "python.exe";
    private const string MyPyExeName = "mypy.exe";
    private const string TempFileName = "temp.py";

    private static string GetPythonExe(RunSpec runSpec) => $"{runSpec.Options.PythonPath}\\{PythonExeName}";

    private static string GetMyPyExe(RunSpec runSpec) => $"{runSpec.Options.PythonPath}\\Scripts\\{MyPyExeName}";

    private static string GetVersion(RunSpec runSpec) {
        var version = Helpers.GetVersion(GetPythonExe(runSpec), "--version", runSpec);
        return string.IsNullOrEmpty(version) ? "not found" : version.Replace("Python ", "").Trim();
    }

    internal static string[] GetNameAndVersion(RunSpec runSpec) => new[] { "python", GetVersion(runSpec) };

    private static (RunResult, string) UpdateLineNumber((RunResult, string) result, RunSpec runSpec) {
        var (rr, _) = result;
        if (rr.outcome == Outcome.CompilationError) {
            try {
                var err = rr.cmpinfo.Split("\n");
                var lineErr = err[0].Split(",")[1].Replace("line", "").Trim();
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

    private static (RunResult, string) UpdateTypeCheckLineNumber((RunResult, string) result, RunSpec runSpec) {
        var (rr, _) = result;
        if (rr.outcome == Outcome.CompilationError) {
            try {
                var err = rr.cmpinfo.Split(":");
                if (int.TryParse(err[1], out var lineNo)) {
                    rr.line_no = Helpers.AdJustCompilerOffset(lineNo, runSpec.Options.LineAdjustment);
                }

                if (int.TryParse(err[2], out var colNo)) {
                    rr.col_no = Helpers.AdJustCompilerOffset(colNo, runSpec.Options.ColumnAdjustment);
                }
            }
            catch (Exception) {
                // ignore all exceptions
            }
        }

        return result;
    }

    internal static (RunResult, string) CompileOrTypeCheck(RunSpec runSpec) =>
        runSpec.Options.PythonUseTypeAnnotations
            ? TypeCheck(runSpec, CreateSourceFile(runSpec), TempFileName)
            : Compile(runSpec, CreateSourceFile(runSpec), TempFileName);

    private static string CreateSourceFile(RunSpec runSpec) {
        var file = $"{runSpec.TempDir}{TempFileName}";
        File.WriteAllText(file, runSpec.sourcecode);
        return file;
    }

    private static (RunResult, string) Compile(RunSpec runSpec, string file, string tempFileName) =>
        UpdateLineNumber(Helpers.Compile(GetPythonExe(runSpec), $"-m py_compile {file}", tempFileName, runSpec), runSpec);

    internal static (RunResult, string) Compile(RunSpec runSpec) => Compile(runSpec, CreateSourceFile(runSpec), TempFileName);

    private static (RunResult, string) TypeCheck(RunSpec runSpec, string file, string tempFileName) =>
        UpdateTypeCheckLineNumber(Helpers.TypeCheck(GetMyPyExe(runSpec), $"{file}  {runSpec.Options.CompileArguments}", tempFileName, runSpec), runSpec);
}