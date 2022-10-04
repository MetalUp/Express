using System.Diagnostics;
using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public class PythonHandler {
    public static Task<ActionResult<RunResult>> CompileAndRun(RunSpec runSpec) {
        return Task.Factory.StartNew(() => {

            var path = System.IO.Path.GetTempPath();
            const string tempFileName = "temp.py";
            var file = path + tempFileName;
            
            File.WriteAllText(file, runSpec.sourcecode);

            var start = new ProcessStartInfo {
                FileName = @"C:\Python310\python.exe",
                Arguments = file,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var runResult = new RunResult();

            try {
                using var process = Process.Start(start);

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