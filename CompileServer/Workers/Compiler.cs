using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers; 

public class Compiler {
    public static Task<ActionResult<RunResult>> Compile(RunSpecWrapper runSpec) {
        return runSpec.run_spec.language_id switch {
            "csharp" => CSharpCompiler.CompileAsTask(runSpec.run_spec),
            _ => Task.Factory.StartNew(() => new ActionResult<RunResult>(new RunResult() {outcome = Outcome.IllegalSystemCall}))
        };
    }
}