using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

[Route("restapi/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TestController : CompileServerController {
    public TestController(ILogger<RunsController> logger, IConfiguration configuration) : base(logger, configuration) { }

    [HttpPost]
    public async Task<ActionResult<RunResult>> Run([FromBody] RunSpecWrapper runSpec) => await Handler.CompileAndTest(runSpec.run_spec);
}