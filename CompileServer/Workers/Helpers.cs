using System.Diagnostics;
using System.Text;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class Helpers {
    private static Process CreateProcess(string file, string args, RunSpec runSpec) {
        var start = new ProcessStartInfo {
            FileName = file,
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = runSpec.TempDir
        };

        return Process.Start(start) ?? throw new NullReferenceException("Process failed to start");
    }

    private static RunResult SetCompileResults(Process process, RunResult runResult) {
        using var stdErr = process.StandardError;
        runResult.cmpinfo = stdErr.ReadToEnd();
        runResult.outcome = string.IsNullOrEmpty(runResult.cmpinfo) ? Outcome.Ok : Outcome.CompilationError;
        return runResult;
    }

    private static RunResult SetTypeCheckResults(Process process, RunResult runResult) {
        using var stdOut = process.StandardOutput;
        var message = stdOut.ReadToEnd();

        if (message.StartsWith("Success")) {
            runResult.outcome = Outcome.Ok;
        }
        else {
            runResult.outcome = Outcome.CompilationError;
            runResult.cmpinfo = message;
        }

        return runResult;
    }

    private static RunResult SetCompileResults(RunResult runResult, Exception e) {
        runResult.outcome = Outcome.CompilationError;
        runResult.stderr = e.Message;
        return runResult;
    }

    private static RunResult SetRunResults(Process process, RunResult runResult) {
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

    private static RunResult SetRunResults(RunResult runResult, Exception e) {
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

    public static RunResult Execute(string exe, string args, RunSpec runSpec, RunResult runResult) {
        try {
            Console.OutputEncoding = Encoding.UTF8;
            using var process = CreateProcess(exe, args, runSpec);
            if (!process.WaitForExit(runSpec.Options.ProcessTimeout)) {
                process.Kill();
            }

            runResult = SetRunResults(process, runResult);
        }
        catch (Exception e) {
            runResult = SetRunResults(runResult, e);
        }

        return runResult;
    }

    public static (RunResult, string) Compile(string exe, string args, string returnFileName, RunSpec runSpec) {
        var runResult = new RunResult();

        try {
            using var process = CreateProcess(exe, args, runSpec);
            process.WaitForExit();
            runResult = SetCompileResults(process, runResult);
        }
        catch (Exception e) {
            runResult = SetCompileResults(runResult, e);
        }

        return (runResult, returnFileName);
    }

    public static (RunResult, string) TypeCheck(string exe, string args, string returnFileName, RunSpec runSpec) {
        var runResult = new RunResult();

        try {
            using var process = CreateProcess(exe, args, runSpec);
            process.WaitForExit();
            runResult = SetTypeCheckResults(process, runResult);
        }
        catch (Exception e) {
            runResult = SetCompileResults(runResult, e);
        }

        return (runResult, returnFileName);
    }

    public static string GetVersion(string exe, string args, RunSpec runSpec) {
        string version;

        try {
            runSpec.SetUp();
            using var process = CreateProcess(exe, args, runSpec);
            process.WaitForExit();
            using var stdOutput = process.StandardOutput;
            version = stdOutput.ReadToEnd();
        }
        catch (Exception e) {
            version = e.Message;
        }
        finally {
            Task.Run(() => Directory.Delete(runSpec.TempDir, true));
        }

        return version;
    }
}