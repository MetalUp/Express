using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

//[ApiController]
[Route("restapi/[controller]")]
public class LanguagesController : ControllerBase {
    private readonly ILogger<LanguagesController> logger;

    public LanguagesController(ILogger<LanguagesController> logger) => this.logger = logger;

    [HttpGet]
    public IEnumerable<string> Get() {
        return new[] { "csharp", "python" };
    }
}