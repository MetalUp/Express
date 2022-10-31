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

    private static readonly MetadataReference[] TestReferences = References.Union(DotNetCompiler.TestReferences).ToArray();

    private static string GetVersion() => Options.LanguageVersion.ToString().Replace("CSharp", "");

    public static string[] GetNameAndVersion() => new[] { "csharp", GetVersion() };

    public static (RunResult, byte[]) Compile(RunSpec runSpec, bool createExecutable) => DotNetCompiler.Compile(runSpec, GenerateGenerateCode(References, true));

    public static (RunResult, byte[]) CompileForTest(RunSpec runSpec) => DotNetCompiler.Compile(runSpec, GenerateGenerateCode(TestReferences, false));

    private static Func<string, CSharpCompilation> GenerateGenerateCode(MetadataReference[] references, bool console) =>
        sourceCode => GenerateCode(sourceCode, references, console);

    private static CSharpCompilation GenerateCode(string sourceCode, MetadataReference[] references, bool console) {
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(SourceText.From(sourceCode), Options);

        return CSharpCompilation.Create("compiled",
                                        new[] { parsedSyntaxTree },
                                        references,
                                        new CSharpCompilationOptions(console ?  OutputKind.ConsoleApplication : OutputKind.DynamicallyLinkedLibrary,
                                                                     optimizationLevel: OptimizationLevel.Release,
                                                                     assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }
}