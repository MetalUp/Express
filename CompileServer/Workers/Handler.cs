using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public static class Handler {
    private static Task<ActionResult<RunResult>> PythonCompileAndRun(RunSpec runSpec) =>
        Task.Run(() => {
                var (runResult, pyFile) = PythonCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = PythonRunner.Execute(pyFile);
                }

                return new ActionResult<RunResult>(runResult);
            }
        );

    private static Task<ActionResult<RunResult>> JavaCompileAndRun(RunSpec runSpec) =>
        Task.Run(() => {
                var (runResult, classFile) = JavaCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = JavaRunner.Execute(classFile);
                }

                return new ActionResult<RunResult>(runResult);
            }
        );

    private static (RunResult, byte[]) DotNetCompile(RunSpec runSpec) =>
        runSpec.language_id switch {
            "csharp" => CSharpCompiler.Compile(runSpec),
            "vb" => VisualBasicCompiler.Compile(runSpec),
            _ => (new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, Array.Empty<byte>())
        };

    private static Task<ActionResult<RunResult>> DotNetCompileAndRun(RunSpec runSpec) =>
        Task.Run(() => {
                var (runResult, assembly) = DotNetCompile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = DotNetRunner.Execute(assembly, runResult);
                }

                return new ActionResult<RunResult>(runResult);
            }
        );

    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec) =>
        runSpec.language_id switch {
            "python" => PythonCompileAndRun(runSpec),
            "java" => JavaCompileAndRun(runSpec),
            "csharp" or "vb" => DotNetCompileAndRun(runSpec),
            _ => Task.Run(() => new ActionResult<RunResult>(new RunResult { outcome = Outcome.IllegalSystemCall }))
        };
}