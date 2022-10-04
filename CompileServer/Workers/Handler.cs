using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public static class Handler {
    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec) =>
        runSpec.language_id switch {
            "python" => PythonHandler.CompileAndRun(runSpec),
            "java" => JavaHandler.CompileAndRun(runSpec),
            _ => DotNetHandler.CompileAndRun(runSpec)
        };
}