using CompileServer.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace CompileServer.Workers;

public static class CSharpCompiler {
    private static readonly MetadataReference[] CSharpReferences = {
        MetadataReference.CreateFromFile(typeof(CSharpArgumentInfo).Assembly.Location)
    };

    private static readonly MetadataReference[] References = DotNetCompiler.DotNetReferences.Union(CSharpReferences).ToArray();
    private static CSharpParseOptions GetOptions(RunSpec runSpec) => CSharpParseOptions.Default.WithLanguageVersion(runSpec.Options.CSharpVersion);

    private static string GetVersion(RunSpec runSpec) => GetOptions(runSpec).LanguageVersion.ToString().Replace("CSharp", "");

    internal static string[] GetNameAndVersion(RunSpec runSpec) => new[] { "csharp", GetVersion(runSpec) };

    internal static (RunResult, byte[]) Compile(RunSpec runSpec) => DotNetCompiler.Compile(runSpec, GenerateGenerateCode(runSpec, References, true));

    internal static (RunResult, byte[]) CompileForTest(RunSpec runSpec) => DotNetCompiler.Compile(runSpec, GenerateGenerateCode(runSpec, References, false));

    private static Func<string, CSharpCompilation> GenerateGenerateCode(RunSpec runSpec, MetadataReference[] references, bool console) =>
        sourceCode => GenerateCode(runSpec, sourceCode, references, console);

    private static CSharpCompilation GenerateCode(RunSpec runSpec, string sourceCode, MetadataReference[] references, bool console) {
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(SourceText.From(sourceCode), GetOptions(runSpec));

        return CSharpCompilation.Create("compiled",
                                        new[] { parsedSyntaxTree },
                                        references,
                                        new CSharpCompilationOptions(console ? OutputKind.ConsoleApplication : OutputKind.DynamicallyLinkedLibrary,
                                                                     optimizationLevel: OptimizationLevel.Release,
                                                                     assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }
}