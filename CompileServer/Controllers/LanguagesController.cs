using CompileServer.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

[Route("restapi/[controller]")]
[Authorize]
public class LanguagesController : CompileServerController {
    public LanguagesController(ILogger<LanguagesController> logger, IConfiguration configuration) : base(logger, configuration) { }

    [HttpGet]
    public IEnumerable<string[]> Get() => new[] {
        new[] { "csharp", CSharpCompiler.GetVersion() },
        new[] { "python", PythonCompiler.GetVersion() },
        new[] { "vb", VisualBasicCompiler.GetVersion() },
        new[] { "java", JavaCompiler.GetVersion() }
    };
}