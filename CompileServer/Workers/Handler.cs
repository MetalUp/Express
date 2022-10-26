using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public static class Handler {
    private static Func<ActionResult<RunResult>> Wrap(Func<ActionResult<RunResult>> f) =>
        () => {
            try {
                return f();
            }
            catch (Exception ex) {
                return new ActionResult<RunResult>(new RunResult { outcome = Outcome.IllegalSystemCall, stderr = ex.Message });
            }
        };

    private static Task<ActionResult<RunResult>> PythonCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, pyFile) = PythonCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = PythonRunner.Execute(pyFile);
                }

                return new ActionResult<RunResult>(runResult);
            })
        );

    private static Task<ActionResult<RunResult>> JavaCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, classFile) = JavaCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = JavaRunner.Execute(classFile);
                }

                return new ActionResult<RunResult>(runResult);
            })
        );

    private static Task<ActionResult<RunResult>> DotNetCompileAndRun(RunSpec runSpec, ILogger logger) =>
        Task.Run(Wrap(() => {
                var (runResult, assembly) = DotNetCompile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = DotNetRunner.Execute(assembly, runResult, logger);
                }

                return new ActionResult<RunResult>(runResult);
            })
        );

    private static Task<ActionResult<RunResult>> DotNetCompileAndTest(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, assembly) = DotNetCompileForTest(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = DotNetTester.Execute(assembly, runResult);
                }

                return new ActionResult<RunResult>(runResult);
            })
        );

    private static (RunResult, byte[]) DotNetCompile(RunSpec runSpec) =>
        runSpec.language_id switch {
            "csharp" => CSharpCompiler.Compile(runSpec),
            "vb" => VisualBasicCompiler.Compile(runSpec),
            _ => (new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, Array.Empty<byte>())
        };

    private static (RunResult, byte[]) DotNetCompileForTest(RunSpec runSpec) =>
        runSpec.language_id switch {
            "csharp" => CSharpCompiler.CompileForTest(runSpec),
            "vb" => VisualBasicCompiler.CompileForTest(runSpec),
            _ => (new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, Array.Empty<byte>())
        };

    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec, ILogger logger) =>
        runSpec.language_id switch {
            "python" => PythonCompileAndRun(runSpec),
            "java" => JavaCompileAndRun(runSpec),
            "csharp" or "vb" => DotNetCompileAndRun(runSpec, logger),
            _ => Task.Run(() => new ActionResult<RunResult>(new RunResult { outcome = Outcome.IllegalSystemCall }))
        };

    public static Task<ActionResult<RunResult>> CompileAndTest(RunSpec runSpec, ILogger logger) =>
        runSpec.language_id switch {
            "csharp" or "vb" => DotNetCompileAndTest(runSpec),
            "python" => PythonCompileAndRun(runSpec),
            _ => Task.Run(() => new ActionResult<RunResult>(new RunResult { outcome = Outcome.IllegalSystemCall }))
        };
}