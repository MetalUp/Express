using System.Collections;
using System.Diagnostics;
using System.Runtime;
using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace CompileServer.Workers;

public static class JavaRunner {
   

    public static RunResult Execute(string classFile) {
       

        var start = new ProcessStartInfo
        {
            FileName = @"C:\Program Files\Java\jdk-17.0.4.1\bin\java.exe",
            Arguments = classFile,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Path.GetTempPath()
    };

        var runResult = new RunResult();

        try
        {
            using var process = Process.Start(start);

            process.WaitForExit();

            using var stdOutput = process.StandardOutput;
            using var stdErr = process.StandardError;
            runResult.stdout = stdOutput.ReadToEnd();
            runResult.stderr = stdErr.ReadToEnd();
            runResult.outcome = string.IsNullOrEmpty(runResult.stderr) ? Outcome.Ok : Outcome.RunTimeError;
        }
        catch (Exception e)
        {
            runResult.outcome = Outcome.RunTimeError;
            runResult.stderr = e.Message;
        }
        finally
        {
            File.Delete($"{Path.GetTempPath()}{classFile}.class");
        }

        return runResult;
    }

  
}