using CompileServer.Controllers;
using CS = Microsoft.CodeAnalysis.CSharp;
using VB = Microsoft.CodeAnalysis.VisualBasic;

namespace CompileServer.Models;

public struct RunSpecWrapper {
    public RunSpecWrapper() { }

    public RunSpec run_spec { get; set; } = new();

    public RestCompileOptions compile_options { get; set; } = new();

    public RunSpec Extract() {
        var rs = run_spec;
        rs.Options = SetDefaults(compile_options);
        return rs;
    }

    private static CS.LanguageVersion ExtractVersion(int? v, CS.LanguageVersion versionDefault) =>
        v is not null && Enum.IsDefined(typeof(CS.LanguageVersion), v) ? (CS.LanguageVersion)v : versionDefault;

    private static VB.LanguageVersion ExtractVersion(int? v, VB.LanguageVersion versionDefault) =>
        v is not null && Enum.IsDefined(typeof(VB.LanguageVersion), v) ? (VB.LanguageVersion)v : versionDefault;

    public static CompileOptions SetDefaults(RestCompileOptions options) {
        var defaultOptions = CompileServerController.GetOptions();
        return new CompileOptions {
            PythonPath = string.IsNullOrWhiteSpace(options.PythonPath) ? defaultOptions.PythonPath : options.PythonPath,
            JavaPath = string.IsNullOrWhiteSpace(options.JavaPath) ? defaultOptions.JavaPath : options.JavaPath,
            HaskellPath = string.IsNullOrWhiteSpace(options.HaskellPath) ? defaultOptions.HaskellPath : options.HaskellPath,
            CSharpVersion = ExtractVersion(options.CSharpVersion, defaultOptions.CSharpVersion),
            VisualBasicVersion = ExtractVersion(options.VisualBasicVersion, defaultOptions.VisualBasicVersion),
            ProcessTimeout = options.ProcessTimeout ?? defaultOptions.ProcessTimeout,
            CompileArguments = options.CompileArguments ?? ""
        };
    }
}