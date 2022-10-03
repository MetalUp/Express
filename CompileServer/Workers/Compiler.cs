using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers; 

public static class Compiler {
    public static Task<ActionResult<RunResult>> Compile(RunSpec runSpec) {
        return runSpec.language_id switch {
            "csharp" => CSharpCompiler.CompileAsTask(runSpec),
            _ => Task.Factory.StartNew(() => new ActionResult<RunResult>(new RunResult() {outcome = Outcome.IllegalSystemCall}))
        };
    }
}