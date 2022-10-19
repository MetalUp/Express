using CompileServer.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using System.Collections;
using System.Runtime;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualBasic.CompilerServices;

namespace CompileServer.Workers;

public static class DotNetCompiler {
    public static (RunResult, byte[]) Compile(RunSpec runSpec, Func<string, Compilation> generateCode) {
        var code = runSpec.sourcecode;

        using var peStream = new MemoryStream();

        var result = generateCode(code).Emit(peStream);

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

    public static readonly MetadataReference[] DotNetReferences = {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(AssemblyTargetedPatchBandAttribute).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location), // System.Linq
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Runtime").Location), // System.Runtime
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Collections").Location), // System.Collections
        MetadataReference.CreateFromFile(typeof(IList<>).Assembly.Location), // System.Collections.Generic
        MetadataReference.CreateFromFile(typeof(ArrayList).Assembly.Location) // System.Collections
    };


    private static readonly MetadataReference[] VBReferences = {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(AssemblyTargetedPatchBandAttribute).Assembly.Location),
        //MetadataReference.CreateFromFile(typeof(VisualBasicCommandLineArguments).Assembly.Location),
        //MetadataReference.CreateFromFile(typeof(StandardModuleAttribute).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location), // System.Linq
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Runtime").Location), // System.Runtime
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("System.Collections").Location), // System.Collections
        MetadataReference.CreateFromFile(typeof(IList<>).Assembly.Location), // System.Collections.Generic
        MetadataReference.CreateFromFile(typeof(ArrayList).Assembly.Location) // System.Collections
    };

}