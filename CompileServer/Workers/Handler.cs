using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public static class Handler {
    private static Func<JsonResult> Wrap(Func<RunResult> f, RunSpec runSpec) =>
        () => {
            try {
                runSpec.SetUp();
                return new JsonResult(f());
            }
            catch (Exception ex) {
                return new JsonResult(new RunResult() { outcome = Outcome.IllegalSystemCall, stderr = ex.Message });
            }
            finally {
                runSpec.CleanUp();
            }
        };

    private static Task<JsonResult> PythonCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, pyFile) = PythonCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = PythonRunner.Execute(pyFile, runSpec,  runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> PythonCompileAndTest(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, pyFile) = PythonCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = PythonTester.Execute(pyFile, runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> PythonCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, _) = PythonCompiler.Compile(runSpec);
                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> JavaCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, classFile) = JavaCompiler.Compile(runSpec, true);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = JavaRunner.Execute(classFile, runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> JavaCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, _) = JavaCompiler.Compile(runSpec, false);
                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> DotNetCompileAndRun(RunSpec runSpec, ILogger logger) =>
        Task.Run(Wrap(() => {
                var (runResult, assembly) = DotNetCompile(runSpec, true);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = DotNetRunner.ExecuteAsProcess(assembly,runSpec, runResult, logger);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> DotNetCompileAndTest(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, assembly) = DotNetCompileForTest(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = DotNetTester.Execute(assembly,runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> DotNetCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, _) = DotNetCompile(runSpec, false);
                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> HaskellCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, exe) = HaskellCompiler.Compile(runSpec, true);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = HaskellRunner.Execute(exe, runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> HaskellCompileAndTest(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, exe) = HaskellCompiler.Compile(runSpec, true);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = HaskellRunner.Execute(exe, runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> HaskellCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, _) = HaskellCompiler.Compile(runSpec, false);
                return runResult;
            }, runSpec)
        );

    private static (RunResult, byte[]) DotNetCompile(RunSpec runSpec, bool createExecutable) =>
        runSpec.language_id switch {
            "csharp" => CSharpCompiler.Compile(runSpec, createExecutable),
            "vb" => VisualBasicCompiler.Compile(runSpec, createExecutable),
            _ => (new RunResult() { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, Array.Empty<byte>())
        };

    private static (RunResult, byte[]) DotNetCompileForTest(RunSpec runSpec) =>
        runSpec.language_id switch {
            "csharp" => CSharpCompiler.CompileForTest(runSpec),
            "vb" => VisualBasicCompiler.CompileForTest(runSpec),
            _ => (new RunResult() { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, Array.Empty<byte>())
        };

    public static Task<JsonResult> Compile(RunSpec runSpec, ILogger logger) =>
        runSpec.language_id switch {
            "python" => PythonCompile(runSpec),
            "java" => JavaCompile(runSpec),
            "csharp" or "vb" => DotNetCompile(runSpec),
            "haskell" => HaskellCompile(runSpec),
            _ => Task.Run(Wrap(() => new RunResult() { outcome = Outcome.IllegalSystemCall }, runSpec))
        };

    public static Task<JsonResult> CompileAndRun(RunSpec runSpec, ILogger logger) =>
        runSpec.language_id switch {
            "python" => PythonCompileAndRun(runSpec),
            "java" => JavaCompileAndRun(runSpec),
            "csharp" or "vb" => DotNetCompileAndRun(runSpec, logger),
            "haskell" => HaskellCompileAndRun(runSpec),
            _ => Task.Run(Wrap(() => new RunResult() { outcome = Outcome.IllegalSystemCall }, runSpec))
        };

    public static Task<JsonResult> CompileAndTest(RunSpec runSpec, ILogger logger) =>
        runSpec.language_id switch {
            "python" => PythonCompileAndTest(runSpec),
            "csharp" or "vb" => DotNetCompileAndTest(runSpec),
            "haskell" => HaskellCompileAndTest(runSpec),
            _ => Task.Run(Wrap(() => new RunResult() { outcome = Outcome.IllegalSystemCall }, runSpec))
        };

    public static string[] GetNameAndVersion(RunSpec runSpec, ILogger logger) =>
        runSpec.language_id switch {
            "python" => PythonCompiler.GetNameAndVersion(runSpec),
            "csharp" => CSharpCompiler.GetNameAndVersion(runSpec),
            "vb" => VisualBasicCompiler.GetNameAndVersion(runSpec),
            "java" => JavaCompiler.GetNameAndVersion(runSpec),
            "haskell" => HaskellCompiler.GetNameAndVersion(runSpec),
            _ => Array.Empty<string>()
        };
}