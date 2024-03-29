﻿using CompileServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Workers;

public static class Handler {
    private static Func<JsonResult> Wrap(Func<RunResult> f, RunSpec runSpec) =>
        () => {
            try {
                runSpec.SetUp();
                return new JsonResult(f());
            }
            catch (Exception ex) {
                return new JsonResult(new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = ex.Message });
            }
            finally {
                runSpec.CleanUp();
            }
        };

    private static Task<JsonResult> PythonCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, pyFile) = PythonCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = PythonRunner.Execute(pyFile, runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> PythonCompileAndTest(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, pyFile) = PythonCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = PythonTester.Execute(pyFile, runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> PythonCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, _) = PythonCompiler.CompileOrTypeCheck(runSpec);
                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> JavaCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, classFile) = JavaCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = JavaRunner.Execute(classFile, runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> JavaCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, _) = JavaCompiler.Compile(runSpec);
                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> CustomCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, csFile) = CustomCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    var csharpRunSpec = new RunSpec() { language_id = "csharp", sourcecode = File.ReadAllText(csFile) };
                    (runResult, var assembly) = DotNetCompileLanguage(runSpec);
                    if (runResult.outcome == Outcome.Ok) {
                        runResult = DotNetRunner.ExecuteAsProcess(assembly, runSpec, runResult);
                    }

                    return runResult;
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> DotNetCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, assembly) = DotNetCompileLanguage(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = DotNetRunner.ExecuteAsProcess(assembly, runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> DotNetCompileAndTest(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, assembly) = DotNetCompileForTest(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = DotNetTester.Execute(assembly, runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> DotNetCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, _) = DotNetCompileLanguage(runSpec);
                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> HaskellCompileAndRun(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, exe) = HaskellCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = HaskellRunner.Execute(exe, runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> HaskellCompileAndTest(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, exe) = HaskellCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    runResult = HaskellTester.Execute(exe, runSpec, runResult);
                }

                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> HaskellCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, _) = HaskellCompiler.Compile(runSpec);
                return runResult;
            }, runSpec)
        );

    private static Task<JsonResult> CustomCompile(RunSpec runSpec) =>
        Task.Run(Wrap(() => {
                var (runResult, csFile) = CustomCompiler.Compile(runSpec);
                if (runResult.outcome == Outcome.Ok) {
                    var csharpRunSpec = new RunSpec() { language_id = "csharp", sourcecode = File.ReadAllText($"{runSpec.TempDir}{csFile}") };
                    (runResult, _) = CSharpCompiler.Compile(csharpRunSpec);
                }
                return runResult;
            }, runSpec)
        );


    private static (RunResult, byte[]) DotNetCompileLanguage(RunSpec runSpec) =>
        runSpec.language_id switch {
            "csharp" => CSharpCompiler.Compile(runSpec),
            "vb" => VisualBasicCompiler.Compile(runSpec),
            _ => (new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, Array.Empty<byte>())
        };

    private static (RunResult, byte[]) DotNetCompileForTest(RunSpec runSpec) =>
        runSpec.language_id switch {
            "csharp" => CSharpCompiler.CompileForTest(runSpec),
            "vb" => VisualBasicCompiler.CompileForTest(runSpec),
            _ => (new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, Array.Empty<byte>())
        };

    public static Task<JsonResult> Compile(RunSpec runSpec) =>
        runSpec.language_id switch {
            "python" => PythonCompile(runSpec),
            "java" => JavaCompile(runSpec),
            "csharp" or "vb" => DotNetCompile(runSpec),
            "haskell" => HaskellCompile(runSpec),
            "bacon" => CustomCompile(runSpec),
            _ => Task.Run(Wrap(() => new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, runSpec))
        };

    public static Task<JsonResult> CompileAndRun(RunSpec runSpec, ILogger logger) =>
        runSpec.language_id switch {
            "python" => PythonCompileAndRun(runSpec),
            "java" => JavaCompileAndRun(runSpec),
            "csharp" or "vb" => DotNetCompileAndRun(runSpec),
            "haskell" => HaskellCompileAndRun(runSpec),
            "bacon" => CustomCompileAndRun(runSpec),
            _ => Task.Run(Wrap(() => new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, runSpec))
        };

    public static Task<JsonResult> CompileAndTest(RunSpec runSpec) =>
        runSpec.language_id switch {
            "python" => PythonCompileAndTest(runSpec),
            "csharp" or "vb" => DotNetCompileAndTest(runSpec),
            "haskell" => HaskellCompileAndTest(runSpec),
            _ => Task.Run(Wrap(() => new RunResult { outcome = Outcome.IllegalSystemCall, cmpinfo = $"Unknown language: {runSpec.language_id}" }, runSpec))
        };

    public static string[] GetNameAndVersion(RunSpec runSpec) =>
        runSpec.language_id switch {
            "python" => PythonCompiler.GetNameAndVersion(runSpec),
            "csharp" => CSharpCompiler.GetNameAndVersion(runSpec),
            "vb" => VisualBasicCompiler.GetNameAndVersion(runSpec),
            "java" => JavaCompiler.GetNameAndVersion(runSpec),
            "haskell" => HaskellCompiler.GetNameAndVersion(runSpec),
            _ => Array.Empty<string>()
        };
}