using CompileServer.Workers;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

[Route("restapi/[controller]")]
public class LanguagesController : CompileServerController {
    public LanguagesController(ILogger<LanguagesController> logger, IConfiguration configuration) : base(logger, configuration) { }

    [HttpGet]
    public IEnumerable<string[]> Get() => new[] {
        new[] { "csharp", CSharpCompiler.GetVersion() },
        new[] { "python", PythonHandler.GetVersion() },
        new[] { "vb", VisualBasicCompiler.GetVersion() },
        new[] { "java", JavaCompiler.GetVersion() }
    };
}