namespace CompileServer.Models;

public struct RunSpec {
    public RunSpec() { }

    public string language_id { get; set; } = "";
    public string sourcecode { get; set; } = "";
}