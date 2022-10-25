using System.Collections;
using System.Reflection;
using System.Runtime;
using System.Security.Cryptography.Xml;
using CompileServer.Models;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompileServer.Workers;

public static class DotNetCompiler {
    private static MetadataReference CopyAndCreate(Assembly asm) {
        var name = asm.GetName().Name;
        var dest = $"{Path.GetTempPath()}{name}.dll";

        if (!File.Exists(dest)) {
            File.Copy(asm.Location, dest);
        }

        return MetadataReference.CreateFromFile(dest);
    }


    public static readonly MetadataReference[] DotNetReferences = {
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Runtime")), // System.Runtime
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Collections")), // System.Collections
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Private.CoreLib")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Runtime")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Console")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("netstandard")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Text.RegularExpressions")), // IMPORTANT!
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Linq")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Linq.Expressions")), // IMPORTANT!
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.IO")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Net.Primitives")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Net.Http")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Private.Uri")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Reflection")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.ComponentModel.Primitives")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Globalization")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Collections.Concurrent")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("System.Collections.NonGeneric")),
        CopyAndCreate(AppDomain.CurrentDomain.Load("Microsoft.CSharp"))
    };

    public static readonly MetadataReference[] TestReferences = {
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("Microsoft.TestPlatform.CommunicationUtilities").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("Microsoft.TestPlatform.CoreUtilities").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("Microsoft.TestPlatform.CrossPlatEngine").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("Microsoft.TestPlatform.PlatformAbstractions").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("Microsoft.TestPlatform.Utilities").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("Microsoft.VisualStudio.TestPlatform.Common").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("Microsoft.VisualStudio.TestPlatform.ObjectModel").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("testhost").Location),

        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("Microsoft.VisualStudio.TestPlatform.TestFramework").Location),
        MetadataReference.CreateFromFile(AppDomain.CurrentDomain.Load("Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions").Location)
    };

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
}