using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

[Route("restapi/[controller]")]
public class RunsController : CompileServerController {
    public RunsController(ILogger<RunsController> logger, IConfiguration configuration) : base(logger, configuration) { }

    [HttpPost]
    public async Task<ActionResult<RunResult>> Run([FromBody] RunSpecWrapper runSpec) => await Handler.CompileAndRun(runSpec.run_spec);
}