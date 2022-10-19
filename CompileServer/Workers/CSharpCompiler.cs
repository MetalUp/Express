using CompileServer.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace CompileServer.Workers;

public static class CSharpCompiler {
    private static readonly CSharpParseOptions Options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);

    private static readonly MetadataReference[] CSharpReferences = {
        MetadataReference.CreateFromFile(typeof(CSharpArgumentInfo).Assembly.Location)
    };

    private static readonly MetadataReference[] References = DotNetCompiler.DotNetReferences.Union(CSharpReferences).ToArray();

    private static string GetVersion() => Options.LanguageVersion.ToString().Replace("CSharp", "");

    public static string[] GetNameAndVersion() => new[] { "csharp", GetVersion() };

    public static (RunResult, byte[]) Compile(RunSpec runSpec) => DotNetCompiler.Compile(runSpec, GenerateCode);

    private static CSharpCompilation GenerateCode(string sourceCode) {
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(SourceText.From(sourceCode), Options);

        return CSharpCompilation.Create("compiled.dll",
                                        new[] { parsedSyntaxTree },
                                        References,
                                        new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                                                                     optimizationLevel: OptimizationLevel.Release,
                                                                     assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }
}