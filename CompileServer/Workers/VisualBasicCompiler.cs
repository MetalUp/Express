using System.Collections;
using System.Runtime;
using CompileServer.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace CompileServer.Workers;

public static class VisualBasicCompiler {
    private static readonly VisualBasicParseOptions Options = VisualBasicParseOptions.Default.WithLanguageVersion(LanguageVersion.VisualBasic16_9);

    private static readonly MetadataReference[] References = {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(AssemblyTargetedPatchBandAttribute).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(VisualBasicCommandLineArguments).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(StandardModuleAttribute).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location), // System.Linq
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Runtime").Location), // System.Runtime
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Collections").Location), // System.Collections
        MetadataReference.CreateFromFile(typeof(IList<>).Assembly.Location), // System.Collections.Generic
        MetadataReference.CreateFromFile(typeof(ArrayList).Assembly.Location) // System.Collections
    };

    public static string GetVersion()
    {
        return Options.LanguageVersion.ToString().Replace("VisualBasic", "");
    }

    public static string[] GetNameAndVersion() => new[] { "vb", GetVersion() };

    public static (RunResult, byte[]) Compile(RunSpec runSpec) {
        var code = runSpec.sourcecode;

        using var peStream = new MemoryStream();

        var result = GenerateCode(code).Emit(peStream);

        if (!result.Success) {
            var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
            return (new RunResult {
                cmpinfo = string.Join('\n', failures.Select(d => d.ToString()).ToArray()),
                outcome = Outcome.CompilationError
            }, Array.Empty<byte>());
        }

        peStream.Seek(0, SeekOrigin.Begin);

        return (new RunResult { outcome = Outcome.Ok }, peStream.ToArray());
    }

    private static VisualBasicCompilation GenerateCode(string sourceCode) {
        var codeString = SourceText.From(sourceCode);

        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, Options);

        return VisualBasicCompilation.Create("compiled.dll",
                                             new[] { parsedSyntaxTree },
                                             References,
                                             new VisualBasicCompilationOptions(OutputKind.ConsoleApplication,
                                                                          optimizationLevel: OptimizationLevel.Release,
                                                                          assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }
}