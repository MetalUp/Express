using CompileServer.Models;
using Microsoft.CodeAnalysis;

namespace CompileServer.Workers;

public static class DotNetCompiler {
    internal static readonly MetadataReference[] DotNetReferences = {
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Runtime").Location), // System.Runtime
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Collections").Location), // System.Collections
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Private.CoreLib").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Runtime").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Console").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("netstandard").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Text.RegularExpressions").Location), // IMPORTANT!
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Linq").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Linq.Expressions").Location), // IMPORTANT!
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.IO").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Net.Primitives").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Net.Http").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Private.Uri").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Reflection").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.ComponentModel.Primitives").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Globalization").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Collections.Concurrent").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Collections.NonGeneric").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("Microsoft.CSharp").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("Microsoft.VisualStudio.TestPlatform.TestFramework").Location)
    };

    internal static (RunResult, byte[]) Compile(RunSpec runSpec, Func<string, Compilation> generateCode) {
        var code = runSpec.sourcecode;

        using var peStream = new MemoryStream();

        var result = generateCode(code).Emit(peStream);

        if (!result.Success) {
            var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
            return (new RunResult(runSpec.TempDir) {
                cmpinfo = new CmpInfo(string.Join('\n', failures.Select(d => d.ToString()).ToArray())),
                outcome = Outcome.CompilationError
            }, Array.Empty<byte>());
        }

        peStream.Seek(0, SeekOrigin.Begin);

        return (new RunResult(runSpec.TempDir) { outcome = Outcome.Ok }, peStream.ToArray());
    }
}