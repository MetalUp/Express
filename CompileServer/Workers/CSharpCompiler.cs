using System.Collections;
using System.Runtime;
using CompileServer.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace CompileServer.Workers;

public static class CSharpCompiler {
    private static readonly CSharpParseOptions Options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);

    private static readonly MetadataReference[] References = {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(AssemblyTargetedPatchBandAttribute).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(CSharpArgumentInfo).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location), // System.Linq
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Runtime").Location), // System.Runtime
        MetadataReference.CreateFromFile(typeof(IList<>).Assembly.Location), // System.Collections.Generic
        MetadataReference.CreateFromFile(typeof(ArrayList).Assembly.Location) // System.Collections
    };

    public static (RunResult, byte[]) Compile(RunSpec runSpec) {
        var code = runSpec.sourcecode;

        using var peStream = new MemoryStream();

        var result = GenerateCode(code).Emit(peStream);

        if (!result.Success) {
            var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
            return (new RunResult {
                cmpinfo = string.Join('\n', failures.Select(d => $"{d.Id}: {d.GetMessage()}").ToArray()),
                outcome = Outcome.CompilationError
            }, Array.Empty<byte>());
        }

        peStream.Seek(0, SeekOrigin.Begin);

        return (new RunResult { outcome = Outcome.Ok }, peStream.ToArray());
    }

    private static CSharpCompilation GenerateCode(string sourceCode) {
        var codeString = SourceText.From(sourceCode);

        var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, Options);

        return CSharpCompilation.Create("compiled.dll",
                                        new[] { parsedSyntaxTree },
                                        References,
                                        new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                                                                     optimizationLevel: OptimizationLevel.Release,
                                                                     assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }
}