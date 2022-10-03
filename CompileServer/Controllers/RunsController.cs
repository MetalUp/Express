using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

//[ApiController]
[Route("restapi/[controller]")]
public class RunsController : ControllerBase {
    private readonly ILogger<RunsController> logger;

    public RunsController(ILogger<RunsController> logger) => this.logger = logger;

    [HttpPost]
    public async Task<ActionResult<RunResult>> Run([FromBody] RunSpecWrapper runSpec) => await Compiler.Compile(runSpec);
}