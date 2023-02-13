using CompileServer.Controllers;

namespace CompileServer.Models;

public struct RunSpecWrapper {
    public RunSpecWrapper() { }

    public RunSpec run_spec { get; set; } = new();

    public RestCompileOptions compile_options { get; set; } = new();

    public RunSpec Extract() {
        var rs = run_spec;
        rs.Options = SetDefaults(compile_options);
        return rs;
    }

    private static CompileOptions SetDefaults(RestCompileOptions options) {
        var defaultOptions = CompileServerController.GetOptions();
        return new CompileOptions {
            PythonPath = string.IsNullOrWhiteSpace(options.PythonPath) ? defaultOptions.PythonPath : options.PythonPath,
            JavaPath = string.IsNullOrWhiteSpace(options.JavaPath) ? defaultOptions.JavaPath : options.JavaPath,
            HaskellPath = string.IsNullOrWhiteSpace(options.HaskellPath) ? defaultOptions.HaskellPath : options.HaskellPath,
            CSharpVersion = options.CSharpVersion ?? defaultOptions.CSharpVersion,
            VisualBasicVersion = options.VisualBasicVersion ?? defaultOptions.VisualBasicVersion,
            ProcessTimeout = options.ProcessTimeout ?? defaultOptions.ProcessTimeout
        };
    }
}