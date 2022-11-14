namespace CompileServer.Models;

public class RunResult {
    internal string TempDir { get; }

    public RunResult(string tempDir) {
        this.TempDir = tempDir;
    }

    public string run_id { get; set; } = "";
    public Outcome outcome { get; set; }
    public string cmpinfo { get; set; } = "";
    public string stdout { get; set; } = "";
    public string stderr { get; set; } = "";
}