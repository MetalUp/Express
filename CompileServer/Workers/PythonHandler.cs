using System.Diagnostics;
using CompileServer.Controllers;
using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;

namespace CompileServer.Workers;

public class PythonHandler {

    public static string GetVersion() {
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";
        string version;

        try {
            using var process = Helpers.CreateProcess(pythonExe, "--version");
            process.WaitForExit();
            using var stdOutput = process.StandardOutput;
            version = stdOutput.ReadToEnd();
        }
        catch (Exception e) {
            version = e.Message;
        }

        return string.IsNullOrEmpty(version) ? "not found" : version.Replace("Python ", "").Trim();
    }


    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec) {
        return Task.Run(() => {
            const string tempFileName = "temp.py";
            var file = $"{System.IO.Path.GetTempPath()}{tempFileName}";
            var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";

            File.WriteAllText(file, runSpec.sourcecode);

            var runResult = new RunResult();

            try {
                using var process = Helpers.CreateProcess(pythonExe, file);

                process.WaitForExit();

                using var stdOutput = process.StandardOutput;
                using var stdErr = process.StandardError;
                runResult.stdout = stdOutput.ReadToEnd();
                runResult.stderr = stdErr.ReadToEnd();
                runResult.outcome = Outcome.Ok;
            }
            catch (Exception e) {
                runResult.outcome = Outcome.RunTimeError;
                runResult.stderr = e.Message;
            }
            finally {
                File.Delete(file);
            }

            return new ActionResult<RunResult>(runResult);
        });
    }
}