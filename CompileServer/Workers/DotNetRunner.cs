using System.Runtime.CompilerServices;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class DotNetRunner {
    public static RunResult Execute(byte[] compiledAssembly, RunResult runResult) {
        var assemblyLoadContextWeakRef = LoadAndExecute(compiledAssembly, runResult);

        Task.Run(() => {
            for (var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++) {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Console.WriteLine(assemblyLoadContextWeakRef.IsAlive ? "Unloading failed!" : "Unloading success!");
        });
        return runResult;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference LoadAndExecute(byte[] compiledAssembly, RunResult runResult) {
        using var asm = new MemoryStream(compiledAssembly);
        var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

        var assembly = assemblyLoadContext.LoadFromStream(asm);

        var entry = assembly.EntryPoint;

        var consoleOut = new StringWriter();
        var consoleErr = new StringWriter();
        var oldOut = Console.Out;
        var oldErr = Console.Error;

        Console.SetOut(consoleOut);
        Console.SetError(consoleErr);

        try {
            entry.Invoke(null, new object[] { Array.Empty<string>() });

            runResult = Helpers.SetRunResults(runResult, consoleOut, consoleErr);
        }
        catch (Exception e) {
            runResult = Helpers.SetRunResults(runResult, e);
        }

        Console.SetOut(oldOut);
        Console.SetError(oldErr);

        assemblyLoadContext.Unload();

        return new WeakReference(assemblyLoadContext);
    }
}