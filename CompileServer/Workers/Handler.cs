using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace CompileServer.Workers;

public static class Handler {
    private static Func<ObjectResult> Wrap(Func<RunResult> f) =>
        () => {
            try {
                var mt = new MediaTypeCollection { "text/plain; charset=utf-8" };
                return new ObjectResult(f()) { ContentTypes = mt };
            }
            catch (Exception ex) {
                return new ObjectResult(new RunResult { outcome = Outcome.IllegalSystemCall, stderr = ex.Message });
            }
        };

    private static Task<ObjectResult> PythonCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, pyFile) = PythonCompiler.Compile(runSpec, true);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = PythonRunner.Execute(pyFile);
                }

                return runResult;
            })
        );

    private static Task<ObjectResult> PythonCompileAndTest(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, pyFile) = PythonCompiler.Compile(runSpec, true);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = PythonTester.Execute(pyFile);
                }

                return runResult;
            })
        );

    private static Task<ObjectResult> PythonCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, _) = PythonCompiler.Compile(runSpec, false);
                return runResult;
            })
        );

    private static Task<ObjectResult> JavaCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, classFile) = JavaCompiler.Compile(runSpec, true);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = JavaRunner.Execute(classFile);
                }

                return runResult;
            })
        );

    private static Task<ObjectResult> JavaCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, _) = JavaCompiler.Compile(runSpec, false);
                return runResult;
            })
        );

    private static Task<ObjectResult> DotNetCompileAndRun(RunSpec runSpec, ILogger logger) =>
        Task.Run(Wrap(() => {
                var (runResult, assembly) = DotNetCompile(runSpec, true);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = DotNetRunner.ExecuteAsProcess(assembly, runResult, logger);
                }

                return runResult;
            })
        );

    private static Task<ObjectResult> DotNetCompileAndTest(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, assembly) = DotNetCompileForTest(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = DotNetTester.Execute(assembly, runResult);
                }

                return runResult;
            })
        );

    private static Task<ObjectResult> DotNetCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, _) = DotNetCompile(runSpec, false);
                return runResult;
            })
        );

    private static (RunResult, byte[]) DotNetCompile(RunSpec runSpec, bool createExecutable) =>
        runSpec.language_id switch {
            "csharp" => CSharpCompiler.Compile(runSpec, createExecutable),
            "vb" => VisualBasicCompiler.Compile(runSpec, createExecutable),
            _ => (new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, Array.Empty<byte>())
        };

    private static (RunResult, byte[]) DotNetCompileForTest(RunSpec runSpec) =>
        runSpec.language_id switch {
            "csharp" => CSharpCompiler.CompileForTest(runSpec),
            "vb" => VisualBasicCompiler.CompileForTest(runSpec),
            _ => (new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, Array.Empty<byte>())
        };

    public static Task<ObjectResult> Compile(RunSpec runSpec, ILogger logger) =>
        runSpec.language_id switch {
            "python" => PythonCompile(runSpec),
            "java" => JavaCompile(runSpec),
            "csharp" or "vb" => DotNetCompile(runSpec),
            _ => Task.Run(Wrap(() => new RunResult { outcome = Outcome.IllegalSystemCall }))
        };

    public static Task<ObjectResult> CompileAndRun(RunSpec runSpec, ILogger logger) =>
        runSpec.language_id switch {
            "python" => PythonCompileAndRun(runSpec),
            "java" => JavaCompileAndRun(runSpec),
            "csharp" or "vb" => DotNetCompileAndRun(runSpec, logger),
            _ => Task.Run(Wrap(() => new RunResult { outcome = Outcome.IllegalSystemCall }))
        };

    public static Task<ObjectResult> CompileAndTest(RunSpec runSpec, ILogger logger) =>
        runSpec.language_id switch {
            "python" => PythonCompileAndTest(runSpec),
            "csharp" or "vb" => DotNetCompileAndTest(runSpec),
            _ => Task.Run(Wrap(() => new RunResult { outcome = Outcome.IllegalSystemCall }))
        };
}