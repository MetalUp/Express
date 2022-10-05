using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

public class CompileServerController : ControllerBase {
    private const string PythonPathName = "PythonPath";
    private const string JavaPathName = "JavaPath";
    private const string CompileServerName = "CompileServer";

    public CompileServerController(IConfiguration configuration) {
        PythonPath = Environment.GetEnvironmentVariable(PythonPathName) ?? configuration.GetSection(CompileServerName).GetSection(PythonPathName).Value;
        JavaPath = Environment.GetEnvironmentVariable(JavaPathName) ?? configuration.GetSection(CompileServerName).GetSection(JavaPathName).Value;
    }

    public static string PythonPath { get; private set; } = "";
    public static string JavaPath { get; private set; } = "";
}