﻿using CompileServer.Models;

namespace CompileServer.Workers;

public static class DotNetRunner {
    internal static RunResult ExecuteAsProcess(byte[] compiledAssembly, RunSpec runSpec, RunResult runResult) {
        try {
            const string tempFileName = "compiled.dll";
            var file = $"{runSpec.TempDir}{tempFileName}";

            File.WriteAllBytes(file, compiledAssembly);

            if (!File.Exists($"{runSpec.TempDir}compiled.runtimeconfig.json")) {
                const string rtg = @"{
                          ""runtimeOptions"": {
                            ""tfm"": ""net6.0"",
                            ""framework"": {
                              ""name"": ""Microsoft.NETCore.App"",
                              ""version"": ""6.0.0""
                            }
                          }
                        }";

                File.WriteAllText($"{runSpec.TempDir}compiled.runtimeconfig.json", rtg);
            }

            var args = $"{file}";

            return Helpers.Execute("dotnet", args, runSpec, runResult);
        }

        catch (Exception e) {
            return Helpers.SetRunResults(runResult, e);
        }
    }
}