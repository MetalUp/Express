using CompileServer.Controllers;

namespace CompileServer.Models;

public class RunSpec {
    public RunSpec() {
        var guid = Guid.NewGuid().ToString();
        TempDir = $"{Path.GetTempPath()}{guid}\\";
    }

    public string language_id { get; set; } = "";
    public string sourcecode { get; set; } = "";

    public CompileOptions Options { get; set; }

    public string TempDir { get; }

    public virtual void SetUp() {
        Directory.CreateDirectory(TempDir);
    }

    public virtual void CleanUp() {
        var tempDir = TempDir;
        Task.Run(() => Directory.Delete(tempDir, true));
    }
}