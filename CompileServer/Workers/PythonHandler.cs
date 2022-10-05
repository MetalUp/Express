using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public static class PythonHandler {
    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec) =>
        Task.Run(() => {
                var (runResult, pyFile) = PythonCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = PythonRunner.Execute(pyFile);
                }

                return new ActionResult<RunResult>(runResult);
            }
        );
}