using CS = Microsoft.CodeAnalysis.CSharp;
using VB = Microsoft.CodeAnalysis.VisualBasic;

namespace CompileServer.Models;

public class RestCompileOptions {
    public string? PythonPath { get; set; }
    public string? CompileArguments { get; set; }
    public string? JavaPath { get; set; }
    public string? HaskellPath { get; set; }
    public int? CSharpVersion { get; set; }
    public int? VisualBasicVersion { get; set; }
    public int? ProcessTimeout { get; set; }
    public int? LineAdjustment { get; set; }
    public int? ColumnAdjustment { get; set; }
}

public class CompileOptions {
    public string PythonPath { get; init; } = "";
    public string CompileArguments { get; set; } = "";
    public string JavaPath { get; init; } = "";
    public string HaskellPath { get; init; } = "";
    public CS.LanguageVersion CSharpVersion { get; init; } = CS.LanguageVersion.CSharp10;
    public VB.LanguageVersion VisualBasicVersion { get; init; } = VB.LanguageVersion.VisualBasic16_9;
    public int ProcessTimeout { get; init; } = 30000;
    public bool PythonUseTypeAnnotations => !string.IsNullOrWhiteSpace(CompileArguments);
    public int LineAdjustment { get; init; } = 0;
    public int ColumnAdjustment { get; init; } = 0;
}