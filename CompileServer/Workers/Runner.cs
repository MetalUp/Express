using CompileServer.Models;

namespace CompileServer.Workers;

public static class Runner {
    public static RunResult Run(RunSpec runSpec, byte[] assembly, RunResult runResult) {
        return runSpec.language_id switch {
            "csharp" => DotNetRunner.Execute(assembly, runResult),
            _ => new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }
        };
    }
}