using CompileServer.Models;

namespace CompileServer.Workers;

public static class DotNetTester {
    private static void LoadIfNotInTemp(string file, RunSpec runSpec) {
        var binPath = @"D:\home\site\wwwroot\TestPlatform\";

        if (!Directory.Exists(binPath)) {
            // for tests
            binPath = @"..\..\..\..\CompileServerTest\bin\Debug\net6.0\";
        }

        if (!File.Exists($"{runSpec.TempDir}{file}")) {
            File.Copy($"{binPath}{file}", $"{runSpec.TempDir}{file}");
        }
    }

    internal static RunResult Execute(byte[] compiledAssembly, RunSpec runSpec, RunResult runResult) {
        try {
            const string tempFileName = "compiled.dll";
            var file = $"{runSpec.TempDir}{tempFileName}";

            File.WriteAllBytes(file, compiledAssembly);

            LoadIfNotInTemp("Microsoft.TestPlatform.AdapterUtilities.dll", runSpec);
            LoadIfNotInTemp("Microsoft.TestPlatform.CommunicationUtilities.dll", runSpec);
            LoadIfNotInTemp("Microsoft.TestPlatform.CoreUtilities.dll", runSpec);
            LoadIfNotInTemp("Microsoft.TestPlatform.CrossPlatEngine.dll", runSpec);
            LoadIfNotInTemp("Microsoft.TestPlatform.PlatformAbstractions.dll", runSpec);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.Common.dll", runSpec);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter.dll", runSpec);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices.dll", runSpec);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices.Interface.dll", runSpec);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.ObjectModel.dll", runSpec);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.TestFramework.dll", runSpec);
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.dll", runSpec);
            LoadIfNotInTemp("Newtonsoft.Json.dll", runSpec);
            LoadIfNotInTemp("NuGet.Frameworks.dll", runSpec);
            LoadIfNotInTemp("testhost.dll", runSpec);

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

            var args = $"test {file} --nologo";

            return Helpers.Execute("dotnet", args, runSpec, runResult);
        }
        catch (Exception e) {
            return Helpers.SetRunResults(runResult, e);
        }
    }
}