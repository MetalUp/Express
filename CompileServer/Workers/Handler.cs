using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers; 

public static class Handler {
    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec) {
        return Task.Factory.StartNew(() => {
                var (runResult, assembly) = Compiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = CSharpRunner.Execute(assembly, runResult);
                }

                return new ActionResult<RunResult>(runResult);
            }
        );
    }
}