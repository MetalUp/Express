using System.Runtime.CompilerServices;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class DotNetRunner {
    public static RunResult Execute(byte[] compiledAssembly, RunResult runResult, ILogger logger) {
        var assemblyLoadContextWeakRef = LoadAndExecute(compiledAssembly, runResult);

        Task.Run(() => {
            for (var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++) {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            if (assemblyLoadContextWeakRef.IsAlive) {
                logger.LogWarning("Unloading failed!");
            }
        });
        return runResult;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference LoadAndExecute(byte[] compiledAssembly, RunResult runResult) {
        using var asm = new MemoryStream(compiledAssembly);
        var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

        var assembly = assemblyLoadContext.LoadFromStream(asm);

        var entry = assembly.EntryPoint;

        if (entry is null) {
            runResult.outcome = Outcome.InternalError;
            runResult.stderr = "No entry point on compiled module";
            return new WeakReference(assemblyLoadContext);
        }

        var consoleOut = new StringWriter();
        var consoleErr = new StringWriter();
        var oldOut = Console.Out;
        var oldErr = Console.Error;

        Console.SetOut(consoleOut);
        Console.SetError(consoleErr);

        try {
            if (entry.GetParameters().Any()) {
                entry.Invoke(null, new object[] { Array.Empty<string>() });
            }
            else {
                entry.Invoke(null, null);
            }

            runResult = Helpers.SetRunResults(runResult, consoleOut, consoleErr);
        }
        catch (Exception e) {
            runResult = Helpers.SetRunResults(runResult, consoleOut, consoleErr, e);
        }

        Console.SetOut(oldOut);
        Console.SetError(oldErr);

        assemblyLoadContext.Unload();

        return new WeakReference(assemblyLoadContext);
    }
}