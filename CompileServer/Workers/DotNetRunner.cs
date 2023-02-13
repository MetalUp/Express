using CompileServer.Models;

namespace CompileServer.Workers;

public static class DotNetRunner {
    internal static RunResult ExecuteAsProcess(byte[] compiledAssembly, RunSpec runSpec,  RunResult runResult, ILogger logger) {
        var consoleOut = new StringWriter();
        var consoleErr = new StringWriter();
        var oldOut = Console.Out;
        var oldErr = Console.Error;

        Console.SetOut(consoleOut);
        Console.SetError(consoleErr);

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
            return Helpers.SetRunResults(runResult, consoleOut, consoleErr, e);
        }
        finally {
            Console.SetOut(oldOut);
            Console.SetError(oldErr);
        }
    }
}