namespace CompileServer.Models;


public class RunResult {
    public RunResult(string tempDir) => TempDir = tempDir;

    internal string TempDir { get; }

    public string run_id { get; set; } = "";
    public Outcome outcome { get; set; }
    public string cmpinfo { get; set; } = "";
    public string stdout { get; set; } = "";
    public string stderr { get; set; } = "";
    public int line_no { get; set; }
    public int col_no { get; set; }
}