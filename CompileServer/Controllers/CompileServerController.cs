using System.Text;
using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;

namespace CompileServer.Controllers;

public abstract class CompileServerController : ControllerBase {
    private const string CompileServerName = "CompileServer";
    private const string PythonPathName = "PythonPath";
    private const string JavaPathName = "JavaPath";
    private const string HaskellPathName = "HaskellPath";
    private const string CSharpVersionName = "CSharpVersion";
    private const string VisualBasicVersionName = "VisualBasicVersion";
    private const string ProcessTimeoutName = "ProcessTimeout";

    protected CompileServerController(ILogger logger, IConfiguration configuration) {
        Logger = logger;

        PythonPath = Environment.GetEnvironmentVariable(PythonPathName) ?? configuration.GetSection(CompileServerName).GetSection(PythonPathName).Value;
        JavaPath = Environment.GetEnvironmentVariable(JavaPathName) ?? configuration.GetSection(CompileServerName).GetSection(JavaPathName).Value;
        HaskellPath = Environment.GetEnvironmentVariable(HaskellPathName) ?? configuration.GetSection(CompileServerName).GetSection(HaskellPathName).Value;

        var csVersion = Environment.GetEnvironmentVariable(CSharpVersionName) ?? configuration.GetSection(CompileServerName).GetSection(CSharpVersionName).Value;
        var vbVersion = Environment.GetEnvironmentVariable(VisualBasicVersionName) ?? configuration.GetSection(CompileServerName).GetSection(VisualBasicVersionName).Value;

        CSharpVersion = Parse<LanguageVersion>(csVersion);
        VisualBasicVersion = Parse<Microsoft.CodeAnalysis.VisualBasic.LanguageVersion>(vbVersion);

        var pto = Environment.GetEnvironmentVariable(ProcessTimeoutName) ?? configuration.GetSection(CompileServerName).GetSection(ProcessTimeoutName).Value;

        if (int.TryParse(pto, out var r)) {
            ProcessTimeout = r;
        }
        System.Console.OutputEncoding = Encoding.UTF8;
    }

    protected ILogger Logger { get; }

    private static string PythonPath { get; set; } = "";
    private static string JavaPath { get; set; } = "";
    private static string HaskellPath { get; set; } = "";
    private static LanguageVersion CSharpVersion { get; set; } = LanguageVersion.CSharp10;
    private static Microsoft.CodeAnalysis.VisualBasic.LanguageVersion VisualBasicVersion { get; set; } = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9;
    private static int ProcessTimeout { get; set; } = 30000;

    private T Parse<T>(string val) where T : struct {
        try {
            return Enum.Parse<T>(val);
        }
        catch (Exception e) {
            Logger.LogError($"Failed to parse: {typeof(T)} value: {val} reason: {e.Message}");
        }

        return default;
    }

    public static CompileOptions GetOptions() =>
        new() {
            PythonPath = PythonPath,
            JavaPath = JavaPath,
            HaskellPath = HaskellPath,
            CSharpVersion = CSharpVersion,
            VisualBasicVersion = VisualBasicVersion,
            ProcessTimeout = ProcessTimeout
        };
}