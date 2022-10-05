using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class PythonRunner {
    public static RunResult Execute(string pyFile) {
       
        var file = $"{Path.GetTempPath()}{pyFile}";
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";

        var runResult = new RunResult();

        try {
            using var process = Helpers.CreateProcess(pythonExe, file);
            process.WaitForExit();
            runResult = Helpers.SetRunResults(process, runResult);
        }
        catch (Exception e) {
            runResult = Helpers.SetRunResults(runResult, e);
        }
        finally {
            File.Delete(file);
        }

        return runResult;
    }
}