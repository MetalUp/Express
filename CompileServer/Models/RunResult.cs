namespace CompileServer.Models;

public class RunResult {
    public string run_id { get; set; } = "";
    public Outcome outcome { get; set; } 
    public string cmpinfo { get; set; } = "";
    public string stdout { get; set; } = "";
    public string stderr { get; set; } = "";
}