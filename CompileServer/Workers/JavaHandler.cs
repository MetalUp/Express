using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public static class JavaHandler {
    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec) =>
        Task.Factory.StartNew(() => {
                var (runResult, assembly) = JavaCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    //runResult = Runner.Run(runSpec, assembly, runResult);
                }

                return new ActionResult<RunResult>(runResult);
            }
        );
}