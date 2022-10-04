using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public static class JavaHandler {
    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec) =>
        Task.Factory.StartNew(() => {
                var (runResult, classFile) = JavaCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = JavaRunner.Execute(classFile);
                }

                return new ActionResult<RunResult>(runResult);
            }
        );
}