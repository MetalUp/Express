using CompileServer.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace CompileServer.Workers;

public static class VisualBasicCompiler {
    private static readonly MetadataReference[] VisualBasicReferences = {
        MetadataReference.CreateFromFile(typeof(VisualBasicCommandLineArguments).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(StandardModuleAttribute).Assembly.Location)
    };

    private static readonly MetadataReference[] References = DotNetCompiler.DotNetReferences.Union(VisualBasicReferences).ToArray();
    private static VisualBasicParseOptions GetOptions(RunSpec runSpec) => VisualBasicParseOptions.Default.WithLanguageVersion(runSpec.Options.VisualBasicVersion);

    private static string GetVersion(RunSpec runSpec) => GetOptions(runSpec).LanguageVersion.ToString().Replace("VisualBasic", "");

    internal static string[] GetNameAndVersion(RunSpec runSpec) => new[] { "vb", GetVersion(runSpec) };

    internal static (RunResult, byte[]) Compile(RunSpec runSpec, bool createExecutable) => DotNetCompiler.Compile(runSpec, GenerateGenerateCode(runSpec, References, true));

    internal static (RunResult, byte[]) CompileForTest(RunSpec runSpec) => DotNetCompiler.Compile(runSpec, GenerateGenerateCode(runSpec, References, false));

    private static Func<string, VisualBasicCompilation> GenerateGenerateCode(RunSpec runSpec, MetadataReference[] references, bool console) =>
        sourceCode => GenerateCode(runSpec, sourceCode, references, console);

    private static VisualBasicCompilation GenerateCode(RunSpec runSpec, string sourceCode, MetadataReference[] references, bool console) {
        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(SourceText.From(sourceCode), GetOptions(runSpec));

        return VisualBasicCompilation.Create("compiled",
                                             new[] { parsedSyntaxTree },
                                             references,
                                             new VisualBasicCompilationOptions(console ? OutputKind.ConsoleApplication : OutputKind.DynamicallyLinkedLibrary,
                                                                               optimizationLevel: OptimizationLevel.Release,
                                                                               assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }
}