using System.Diagnostics;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class JavaCompiler {
    public static (RunResult, string) Compile(RunSpec runSpec) {
        var path = Path.GetTempPath();
        const string tempFileName = "temp.java";
        var file = path + tempFileName;

        File.WriteAllText(file, runSpec.sourcecode);

        var start = new ProcessStartInfo {
            FileName = @"C:\Program Files\Java\jdk-17.0.4.1\bin\javac.exe",
            Arguments = file,
            UseShellExecute = false,
            RedirectStandardError = true
        };

        var runResult = new RunResult();

        try {
            using var process = Process.Start(start);

            process.WaitForExit();

            using var stdErr = process.StandardError;
            runResult.cmpinfo = stdErr.ReadToEnd();
            runResult.outcome = string.IsNullOrEmpty(runResult.cmpinfo) ? Outcome.Ok : Outcome.CompilationError;
        }
        catch (Exception e) {
            runResult.outcome = Outcome.CompilationError;
            runResult.stderr = e.Message;
        }
        finally {
            File.Delete(file);
        }

        return (runResult, path + "temp.class");
    }
}