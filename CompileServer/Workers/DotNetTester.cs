using CompileServer.Models;

namespace CompileServer.Workers;

public static class DotNetTester {
    private static void LoadIfNotInTemp(string file) {
        var home = AppDomain.CurrentDomain.BaseDirectory;
        var binPath = @$".\{home}\TestPlatForm\";

        if (!Directory.Exists(binPath)) {
            binPath = @$"..\..\..\..\CompileServerTest\bin\Debug\net6.0\";
        }

        if (!File.Exists($"{Path.GetTempPath()}{file}")) {
            File.Copy($"{binPath}{file}", $"{Path.GetTempPath()}{file}");
        }
    }

    public static RunResult Execute(byte[] compiledAssembly, RunResult runResult) {
        var consoleOut = new StringWriter();
        var consoleErr = new StringWriter();
        var oldOut = Console.Out;
        var oldErr = Console.Error;

        Console.SetOut(consoleOut);
        Console.SetError(consoleErr);

        try {
            const string tempFileName = "compiled.dll";
            var file = $"{Path.GetTempPath()}{tempFileName}";

            File.WriteAllBytes(file, compiledAssembly);

            LoadIfNotInTemp("testhost.dll");
            LoadIfNotInTemp("Microsoft.TestPlatform.CoreUtilities.dll");
            LoadIfNotInTemp("Microsoft.TestPlatform.PlatformAbstractions.dll");
            LoadIfNotInTemp("Microsoft.TestPlatform.CrossPlatEngine.dll");
            LoadIfNotInTemp("Microsoft.TestPlatform.CommunicationUtilities.dll");
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.ObjectModel.dll");
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.Common.dll");
            LoadIfNotInTemp("Newtonsoft.Json.dll");
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.TestFramework.dll");
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter.dll");
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices.dll");
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices.Interface.dll");
            LoadIfNotInTemp("Microsoft.TestPlatform.AdapterUtilities.dll");
            LoadIfNotInTemp("Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.dll");
            LoadIfNotInTemp("NuGet.Frameworks.dll");

            if (!File.Exists($"{Path.GetTempPath()}compiled.runtimeconfig.json")) {
                var rtg = @"{
                          ""runtimeOptions"": {
                            ""tfm"": ""net6.0"",
                            ""framework"": {
                              ""name"": ""Microsoft.NETCore.App"",
                              ""version"": ""6.0.0""
                            }
                          }
                        }";

                File.WriteAllText($"{Path.GetTempPath()}compiled.runtimeconfig.json", rtg);
            }

            var args = $"test {file} --nologo";

            return Helpers.Execute("dotnet", args, file);
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