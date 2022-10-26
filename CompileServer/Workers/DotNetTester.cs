using CompileServer.Models;

namespace CompileServer.Workers;

public static class DotNetTester {
    private static void LoadIfNotInTemp(string file) {
        var home = AppDomain.CurrentDomain.BaseDirectory;
        var binPath = @$".\{home}\TestPlatForm\";

        if (!Directory.Exists(binPath)) {
            binPath = @$"..\..\..\..\CompileServer\TestPlatForm\";
        }

        if (!File.Exists($"{Path.GetTempPath()}{file}")) {
            File.Copy($"{binPath}{file}", $"{Path.GetTempPath()}{file}");
        }
    }

    public static RunResult Execute(byte[] compiledAssembly, RunResult runResult) {
        const string tempFileName = "compiled.dll";
        var file = $"{Path.GetTempPath()}{tempFileName}";

        File.WriteAllBytes(file, compiledAssembly);

        LoadIfNotInTemp("Microsoft.TestPlatform.CoreUtilities.dll");

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
}