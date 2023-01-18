namespace CompileServer.Models;


public class RunResult {
    public RunResult(string tempDir) => TempDir = tempDir;

    internal string TempDir { get; }

    public string run_id { get; set; } = "";
    public Outcome outcome { get; set; }
    public string cmpinfo { get; set; } = "";
    public string stdout { get; set; } = "";
    public string stderr { get; set; } = "";

    public int LineNo { get; set; }
    public int ColNo { get; set; }
}