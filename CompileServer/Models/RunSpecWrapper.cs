namespace CompileServer.Models;

public struct RunSpecWrapper {
    public RunSpecWrapper() { }

    public RunSpec run_spec { get; set; } = new();
}