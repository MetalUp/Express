using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class JavaRunner {
    public static RunResult Execute(string classFile) {
        var java = $"{CompileServerController.JavaPath}\\bin\\java.exe";
        var runResult = new RunResult();

        try {
            using var process = Helpers.CreateProcess(java, classFile);

            process.WaitForExit();

            runResult = Helpers.SetRunResults(process, runResult);
        }
        catch (Exception e) {
            runResult = Helpers.SetRunResults(runResult, e);
        }
        finally {
            File.Delete($"{Path.GetTempPath()}{classFile}.class");
        }

        return runResult;
    }
}