using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

[Route("restapi/[controller]")]
[Authorize]
public class RunsController : CompileServerController {
    public RunsController(ILogger<RunsController> logger, IConfiguration configuration) : base(logger, configuration) { }

    [HttpPost]
    public async Task<ActionResult<RunResult>> Run([FromBody] RunSpecWrapper runSpec) => await Handler.CompileAndRun(runSpec.run_spec);
}