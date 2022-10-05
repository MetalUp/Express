﻿using System.Diagnostics;
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

    public static RunResult SetCompileResults(RunResult runResult, Exception e)
    {
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

  
}