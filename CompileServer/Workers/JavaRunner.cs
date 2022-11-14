using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class JavaRunner {
    internal static RunResult Execute(string classFile, RunResult runResult) {
        var java = $"{CompileServerController.JavaPath}\\bin\\java.exe";
        return Helpers.Execute(java, classFile, runResult);
    }
}