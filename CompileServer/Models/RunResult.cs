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

public class RunResult {
    public string run_id { get; set; } = "";
    public Outcome outcome { get; set; } 
    public string cmpinfo { get; set; } = "";
    public string stdout { get; set; } = "";
    public string stderr { get; set; } = "";
}