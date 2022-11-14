using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

[Route("restapi/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LanguagesController : CompileServerController {
    public LanguagesController(ILogger<LanguagesController> logger, IConfiguration configuration) : base(logger, configuration) { }

    [HttpGet]
    public IEnumerable<string[]> Get() => new[] { "csharp", "vb", "python", "java" }.Select(l => Handler.GetNameAndVersion(new RunSpec { language_id = l }, Logger));
}