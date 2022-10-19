using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class JavaRunner {
    public static RunResult Execute(string classFile) {
        var java = $"{CompileServerController.JavaPath}\\bin\\java.exe";
        var file = $"{Path.GetTempPath()}{classFile}.class";
        return Helpers.Execute(java, classFile, file);
    }
}