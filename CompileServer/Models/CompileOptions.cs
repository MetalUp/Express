using Microsoft.CodeAnalysis.CSharp;

namespace CompileServer.Models;

public class RestCompileOptions {
    public string PythonPath { get; set; } = "";
    public string MyPyArguments { get; set; } = "";
    public string JavaPath { get; set; } = "";
    public string HaskellPath { get; set; } = "";
    public LanguageVersion? CSharpVersion { get; set; } = LanguageVersion.CSharp10;
    public Microsoft.CodeAnalysis.VisualBasic.LanguageVersion? VisualBasicVersion { get; set; } = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9;
    public int? ProcessTimeout { get; set; } = 30000;
}

public class CompileOptions {
    public string PythonPath { get; set; } = "";
    public string MyPyArguments { get; set; } = "";
    public string JavaPath { get; set; } = "";
    public string HaskellPath { get; set; } = "";
    public LanguageVersion CSharpVersion { get; set; } = LanguageVersion.CSharp10;
    public Microsoft.CodeAnalysis.VisualBasic.LanguageVersion VisualBasicVersion { get; set; } = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9;
    public int ProcessTimeout { get; set; } = 30000;
    public bool PythonUseTypeAnnotations => !string.IsNullOrWhiteSpace(MyPyArguments);
}