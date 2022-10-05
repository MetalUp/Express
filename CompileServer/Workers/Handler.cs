using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public static class Handler {
    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec) =>
        runSpec.language_id switch {
            "python" => PythonHandler.CompileAndRun(runSpec),
            "java" => JavaHandler.CompileAndRun(runSpec),  
            "csharp" => DotNetHandler.CompileAndRun(runSpec),
            "vb" => DotNetHandler.CompileAndRun(runSpec),
            _ => Task.Run(() => new ActionResult<RunResult>(new RunResult() {outcome = Outcome.IllegalSystemCall})) 
        };
}