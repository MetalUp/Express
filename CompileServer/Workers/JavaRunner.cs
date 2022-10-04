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
   

    public static RunResult Execute(string execPath, RunSpec runSpec) {
        var path = System.IO.Path.GetTempPath();
       
        var file = path + execPath;

        var start = new ProcessStartInfo
        {
            FileName = @"C:\Program Files\Java\jdk-17.0.4.1\bin\java.exe",
            Arguments = file,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
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
            runResult.outcome = Outcome.Ok;
        }
        catch (Exception e)
        {
            runResult.outcome = Outcome.RunTimeError;
            runResult.stderr = e.Message;
        }
        finally
        {
            File.Delete(file);
        }

        return runResult;
    }

  
}