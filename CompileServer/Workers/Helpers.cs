using System.Diagnostics;

namespace CompileServer.Workers;

public static class Helpers {
    public static Process CreateProcess(string file, string args) {
        var start = new ProcessStartInfo {
            FileName = file,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Path.GetTempPath()
        };

        return Process.Start(start) ?? throw new NullReferenceException("Process failed to start");
    }
}