using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

[Route("restapi/[controller]")]
public class RunsController : CompileServerController {
    private readonly ILogger<RunsController> logger;

    public RunsController(ILogger<RunsController> logger, IConfiguration configuration) : base(configuration) => this.logger = logger;

    [HttpPost]
    public async Task<ActionResult<RunResult>> Run([FromBody] RunSpecWrapper runSpec) => await Handler.CompileAndRun(runSpec.run_spec);
}