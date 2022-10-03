namespace CompileServer.Models;

public class RunSpec
{
    public string language_id { get; set; } = "";
    public string sourcecode { get; set; } = "";
}


public class RunSpecWrapper {
    public RunSpec run_spec { get; set; }
}