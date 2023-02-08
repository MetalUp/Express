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
    private const string PythonUseTypeAnnotationsName = "PythonUseTypeAnnotations";

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

        var pta = Environment.GetEnvironmentVariable(PythonUseTypeAnnotationsName) ?? configuration.GetSection(CompileServerName).GetSection(PythonUseTypeAnnotationsName).Value;

        if (bool.TryParse(pta, out var b)) {
            PythonUseTypeAnnotations = b;
        }
    }

    protected ILogger Logger { get; }

    public static string PythonPath { get; set; } = "";
    public static string JavaPath { get; set; } = "";
    public static string HaskellPath { get;  set; } = "";
    public static LanguageVersion CSharpVersion { get; private set; } = LanguageVersion.CSharp10;
    public static Microsoft.CodeAnalysis.VisualBasic.LanguageVersion VisualBasicVersion { get; private set; } = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9;
    public static int ProcessTimeout { get; private set; } = 30000;
    public static bool PythonUseTypeAnnotations { get; set; } = true;

    private T Parse<T>(string val) where T : struct {
        try {
            return Enum.Parse<T>(val);
        }
        catch (Exception e) {
            Logger.LogError($"Failed to parse: {typeof(T)} value: {val} reason: {e.Message}");
        }

        return default;
    }
}