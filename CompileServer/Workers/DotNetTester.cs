using System.Collections;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using CompileServer.Controllers;
using CompileServer.Models;
using Microsoft.CodeAnalysis;

namespace CompileServer.Workers;

public static class DotNetTester {

    private static void LoadIfNotInTemp(string file) {
        var binPath = @"C:\GitHub\ILE\CompileServerTest\bin\Debug\net6.0\";
        if (!File.Exists($"{(string?)Path.GetTempPath()}{file}"))
        {
            File.Copy($"{binPath}{file}", $"{Path.GetTempPath()}{file}");
        }
    }

    //private static void SystemLoadIfNotInTemp(Assembly asm) {
    //    var name = asm.GetName().Name;
    //    File.Delete($"{Path.GetTempPath()}{name}.dll");
    //    //if (!File.Exists($"{(string?)Path.GetTempPath()}{name}")) {
    //    //    File.Copy(asm.Location, $"{Path.GetTempPath()}{name}.dll");
    //    //}
    //}

    //private static void SystemLoadIfNotInTemp(Assembly asm)
    //{
    //    var name = asm.GetName().Name;
    //    File.Delete($"{Path.GetTempPath()}{name}.dll");
    //    //if (!File.Exists($"{Path.GetTempPath()}{name}.dll"))
    //    //{
    //    //    File.Copy(asm.Location, $"{Path.GetTempPath()}{name}.dll");
    //    //}
    //}



    public static RunResult Execute(byte[] compiledAssembly, RunResult runResult) {
     
        const string tempFileName = "SimpleTest.dll";
        var file = $"{Path.GetTempPath()}{tempFileName}";

        File.Delete(file);
        
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

        //SystemLoadIfNotInTemp(typeof(object).Assembly);
        //SystemLoadIfNotInTemp(typeof(Console).Assembly);
        //SystemLoadIfNotInTemp(typeof(AssemblyTargetedPatchBandAttribute).Assembly);
        //SystemLoadIfNotInTemp(typeof(Enumerable).Assembly);
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Runtime"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Collections"));
        //SystemLoadIfNotInTemp(typeof(IList<>).Assembly);
        //SystemLoadIfNotInTemp(typeof(ArrayList).Assembly);
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Private.CoreLib"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Private.Uri"));


        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Runtime")); // System.Runtime
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Collections")); // System.Collections
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Private.CoreLib"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Runtime"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Console"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("netstandard"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Text.RegularExpressions")); // IMPORTANT!
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Linq"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Linq.Expressions")); // IMPORTANT!
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.IO"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Net.Primitives"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Net.Http"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Private.Uri"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Reflection"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.ComponentModel.Primitives"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Globalization"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Collections.Concurrent"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("System.Collections.NonGeneric"));
        //SystemLoadIfNotInTemp(AppDomain.CurrentDomain.Load("Microsoft.CSharp"));

        if (!File.Exists($"{Path.GetTempPath()}SimpleTest.runtimeconfig.json"))
        {
            File.Copy(@"C:\GitHub\ILE\SimpleTest\bin\Debug\net6.0\SimpleTest.runtimeconfig.json", $"{Path.GetTempPath()}SimpleTest.runtimeconfig.json");
        }

        var args = $"test {file} --nologo";

        return Helpers.Execute("dotnet", args, file);
    }
}

