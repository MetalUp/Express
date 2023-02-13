using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class JavaRunner {
    internal static RunResult Execute(string classFile, RunSpec runSpec, RunResult runResult) {
        var java = $"{runSpec.Options.JavaPath}\\bin\\java.exe";
        return Helpers.Execute(java, classFile, runSpec, runResult);
    }
}