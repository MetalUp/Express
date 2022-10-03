namespace CompileServer.Models;

public enum Outcome {
    CompilationError = 11,
    RunTimeError = 12,
    TimeLimitExceeded = 13,
    Ok = 15,
    MemoryLimitExceeded = 17,
    IllegalSystemCall = 19,
    InternalError = 20,
    ServerOverload = 21
}