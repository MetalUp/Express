using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public static class DotNetHandler {
    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec) =>
        Task.Run(() => {
                var (runResult, assembly) = Compiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = Runner.Run(runSpec, assembly, runResult);
                }

                return new ActionResult<RunResult>(runResult);
            }
        );
}