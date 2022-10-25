using System.Diagnostics;
using CompileServer.Models;

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

    public static RunResult SetCompileResults(Process process, RunResult runResult) {
        using var stdErr = process.StandardError;
        runResult.cmpinfo = stdErr.ReadToEnd();
        runResult.outcome = string.IsNullOrEmpty(runResult.cmpinfo) ? Outcome.Ok : Outcome.CompilationError;
        return runResult;
    }

    public static RunResult SetCompileResults(RunResult runResult, Exception e) {
        runResult.outcome = Outcome.CompilationError;
        runResult.stderr = e.Message;
        return runResult;
    }

    public static RunResult SetRunResults(Process process, RunResult runResult) {
        using var stdOutput = process.StandardOutput;
        using var stdErr = process.StandardError;
        runResult.stdout = stdOutput.ReadToEnd();
        runResult.stderr = stdErr.ReadToEnd();
        runResult.outcome = string.IsNullOrEmpty(runResult.stderr) ? Outcome.Ok : Outcome.RunTimeError;
        return runResult;
    }

    public static RunResult SetRunResults(RunResult runResult, StringWriter consoleOut, StringWriter consoleErr) {
        runResult.stdout = consoleOut.ToString();
        runResult.stderr = consoleErr.ToString();
        runResult.outcome = string.IsNullOrEmpty(runResult.stderr) ? Outcome.Ok : Outcome.RunTimeError;
        return runResult;
    }

    public static RunResult SetRunResults(RunResult runResult, Exception e) {
        runResult.outcome = Outcome.RunTimeError;
        runResult.stderr = e.Message;
        return runResult;
    }

    private static string GetInnerMostMessage(Exception e) => e.InnerException is not null ? GetInnerMostMessage(e.InnerException) : e.Message;

    public static RunResult SetRunResults(RunResult runResult, StringWriter consoleOut, StringWriter consoleErr, Exception e) {
        runResult.outcome = Outcome.RunTimeError;
        runResult.stdout = consoleOut.ToString();
        var err = consoleErr.ToString();
        runResult.stderr = string.IsNullOrEmpty(err) ? GetInnerMostMessage(e) : err;
        return runResult;
    }

    public static RunResult Execute(string exe, string args, string cleanUp) {
        var runResult = new RunResult();

        try {
            using var process = CreateProcess(exe, args);
            process.WaitForExit();
            runResult = SetRunResults(process, runResult);
        }
        catch (Exception e) {
            runResult = SetRunResults(runResult, e);
        }
        finally {
            if (!string.IsNullOrEmpty(cleanUp)) {
                File.Delete(cleanUp);
            }
        }

        return runResult;
    }

    public static (RunResult, string) Compile(string exe, string args, string cleanUp, string returnFileName) {
        var runResult = new RunResult();

        try {
            using var process = CreateProcess(exe, args);
            process.WaitForExit();
            runResult = SetCompileResults(process, runResult);
        }
        catch (Exception e) {
            runResult = SetCompileResults(runResult, e);
        }
        finally {
            if (!string.IsNullOrEmpty(cleanUp)) {
                File.Delete(cleanUp);
            }
        }

        return (runResult, returnFileName);
    }

    public static string GetVersion(string exe, string args) {
        string version;

        try {
            using var process = CreateProcess(exe, args);
            process.WaitForExit();
            using var stdOutput = process.StandardOutput;
            version = stdOutput.ReadToEnd();
        }
        catch (Exception e) {
            version = e.Message;
        }

        return version;
    }
}