using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

[Route("restapi/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CompilesController : CompileServerController {
    public CompilesController(ILogger<RunsController> logger, IConfiguration configuration) : base(logger, configuration) { }

    [HttpPost]
    public async Task<ActionResult> Run([FromBody] RunSpecWrapper runSpec) => await Handler.Compile(runSpec.run_spec, Logger);
}