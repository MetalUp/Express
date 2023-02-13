using Microsoft.CodeAnalysis.CSharp;

namespace CompileServer.Models;

public class CompileOptions {
    public string PythonPath { get; init; } = "";
    public string JavaPath { get; init; } = "";
    public string HaskellPath { get; init; } = "";
    public LanguageVersion CSharpVersion { get; init; } = LanguageVersion.CSharp10;
    public Microsoft.CodeAnalysis.VisualBasic.LanguageVersion VisualBasicVersion { get; init; } = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9;
    public int ProcessTimeout { get; init; } = 30000;
    public bool PythonUseTypeAnnotations { get; set; } = true;
}