using CompileServer.Models;

namespace CompileServer.Workers;

public static class Compiler {
    public static (RunResult, byte[]) Compile(RunSpec runSpec) {
        return runSpec.language_id switch {
            "csharp" => CSharpCompiler.Compile(runSpec),
            _ => (new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}"}, Array.Empty<byte>())
        };
    }
}