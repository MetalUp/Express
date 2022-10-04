using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public static class JavaHandler {
    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec) =>
        Task.Factory.StartNew(() => {
                var (runResult, execPath) = JavaCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = JavaRunner.Execute(execPath);
                }

                return new ActionResult<RunResult>(runResult);
            }
        );
}