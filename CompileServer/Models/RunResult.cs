namespace CompileServer.Models;

public class CmpInfo {

    public CmpInfo(string message) {
        Message = message;
    }

    public string Message { get; set; }
    public int? LineNo { get; set; }
    public int? ColNo { get; set; }
}

public class RunResult {
    public RunResult(string tempDir) => TempDir = tempDir;

    internal string TempDir { get; }

    public string run_id { get; set; } = "";
    public Outcome outcome { get; set; }
    public CmpInfo cmpinfo { get; set; } = new CmpInfo("");
    public string stdout { get; set; } = "";
    public string stderr { get; set; } = "";
}