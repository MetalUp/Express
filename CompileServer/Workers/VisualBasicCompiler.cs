using CompileServer.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace CompileServer.Workers;

public static class VisualBasicCompiler {
    private static readonly VisualBasicParseOptions Options = VisualBasicParseOptions.Default.WithLanguageVersion(LanguageVersion.VisualBasic16_9);

    private static readonly MetadataReference[] VisualBasicReferences = {
        MetadataReference.CreateFromFile(typeof(VisualBasicCommandLineArguments).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(StandardModuleAttribute).Assembly.Location)
    };

    private static readonly MetadataReference[] References = DotNetCompiler.DotNetReferences.Union(VisualBasicReferences).ToArray();

    private static string GetVersion() => Options.LanguageVersion.ToString().Replace("VisualBasic", "");

    public static string[] GetNameAndVersion() => new[] { "vb", GetVersion() };

    public static (RunResult, byte[]) Compile(RunSpec runSpec, bool createExecutable) => DotNetCompiler.Compile(runSpec, GenerateGenerateCode(References, true));

    public static (RunResult, byte[]) CompileForTest(RunSpec runSpec) => DotNetCompiler.Compile(runSpec, GenerateGenerateCode(References, false));

    private static Func<string, VisualBasicCompilation> GenerateGenerateCode(MetadataReference[] references, bool console) =>
        sourceCode => GenerateCode(sourceCode, references, console);

    private static VisualBasicCompilation GenerateCode(string sourceCode, MetadataReference[] references, bool console) {
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(SourceText.From(sourceCode), Options);

        return VisualBasicCompilation.Create("compiled",
                                             new[] { parsedSyntaxTree },
                                             references,
                                             new VisualBasicCompilationOptions(console ? OutputKind.ConsoleApplication : OutputKind.DynamicallyLinkedLibrary,
                                                                               optimizationLevel: OptimizationLevel.Release,
                                                                               assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }
}