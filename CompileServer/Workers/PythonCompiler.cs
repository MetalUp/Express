﻿using System.Linq.Expressions;
using CompileServer.Controllers;
using CompileServer.Models;

namespace CompileServer.Workers;

public static class PythonCompiler {
    private static string GetVersion(RunSpec runSpec) {
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";
        var version = Helpers.GetVersion(pythonExe, "--version", runSpec);

        return string.IsNullOrEmpty(version) ? "not found" : version.Replace("Python ", "").Trim();
    }

    internal static string[] GetNameAndVersion(RunSpec runSpec) => new[] { "python", GetVersion(runSpec) };

    private static (RunResult, string) UpdateLineNumber((RunResult, string) result) {
        var (rr, s) = result;
        if (rr.outcome == Outcome.CompilationError) {
            try {
                var err = rr.cmpinfo.Split("\n");
                var lineErr = err[0].Split(",")[1].Replace("line", "").Trim();
                if (int.TryParse(lineErr, out var lineNo)) {
                    rr.line_no = lineNo;
                }

                var colErr = err[2];
                rr.col_no = colErr[..(colErr.IndexOf('^') + 1)].Length;
            }
            catch (Exception _) {
                // ignore all exceptions
            }
        }

        return result;
    }

    internal static (RunResult, string) Compile(RunSpec runSpec, bool createExecutable) {
        const string tempFileName = "temp.py";
        var file = $"{runSpec.TempDir}{tempFileName}";
        File.WriteAllText(file, runSpec.sourcecode);
        var pythonExe = $"{CompileServerController.PythonPath}\\python.exe";
        var args = $"-m py_compile {file}";

        return UpdateLineNumber(Helpers.Compile(pythonExe, args, createExecutable ? tempFileName : "", runSpec));
    }
}