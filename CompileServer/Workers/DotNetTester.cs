using CompileServer.Models;

namespace CompileServer.Workers;

public static class DotNetTester {
    private static void LoadIfNotInTemp(string file, RunResult runResult) {
        var binPath = @"D:\home\site\wwwroot\TestPlatform\";

        if (!Directory.Exists(binPath)) {
            // for tests
            binPath = @"..\..\..\..\CompileServerTest\bin\Debug\net6.0\";
        }

        if (!File.Exists($"{runResult.TempDir}{file}")) {
            File.Copy($"{binPath}{file}", $"{runResult.TempDir}{file}");
        }
    }

    internal static RunResult Execute(byte[] compiledAssembly, RunResult runResult) {
        var consoleOut = new StringWriter();
        var consoleErr = new StringWriter();
        var oldOut = Console.Out;
        var oldErr = Console.Error;

        Console.SetOut(consoleOut);
        Console.SetError(consoleErr);

        try {
            const string tempFileName = "compiled.dll";
            var file = $"{runResult.TempDir}{tempFileName}";

            File.WriteAllBytes(file, compiledAssembly);

            LoadIfNotInTemp("Microsoft.TestPlatform.AdapterUtilities.dll", runResult);
            LoadIfNotInTemp("Microsoft.TestPlatform.CommunicationUtilities.dll", runResult);
            LoadIfNotInTemp("Microsoft.TestPlatform.CoreUtilities.dll", runResult);
            LoadIfNotInTemp("Microsoft.TestPlatform.CrossPlatEngine.dll", runResult);
            LoadIfNotInTemp("Microsoft.TestPlatform.PlatformAbstractions.dll", runResult);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.Common.dll", runResult);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter.dll", runResult);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices.dll", runResult);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices.Interface.dll", runResult);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.ObjectModel.dll", runResult);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.TestFramework.dll", runResult);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.dll", runResult);
            LoadIfNotInTemp("Newtonsoft.Json.dll", runResult);
            LoadIfNotInTemp("NuGet.Frameworks.dll", runResult);
            LoadIfNotInTemp("testhost.dll", runResult);

            if (!File.Exists($"{runResult.TempDir}compiled.runtimeconfig.json")) {
                var rtg = @"{
                          ""runtimeOptions"": {
                            ""tfm"": ""net6.0"",
                            ""framework"": {
                              ""name"": ""Microsoft.NETCore.App"",
                              ""version"": ""6.0.0""
                            }
                          }
                        }";

                File.WriteAllText($"{runResult.TempDir}compiled.runtimeconfig.json", rtg);
            }

            var args = $"test {file} --nologo";

            return Helpers.Execute("dotnet", args, runResult);
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