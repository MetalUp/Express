using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

[Route("restapi/[controller]")]
public class RunsController : ControllerBase {
    private readonly ILogger<RunsController> logger;
    public static string PythonPath { get; set; } = "";
    public static string JavaPath { get; set; } = "";

    public RunsController(ILogger<RunsController> logger, IConfiguration configuration) {
        this.logger = logger;

        PythonPath = configuration.GetSection("CompileServer").GetSection("PythonPath").Value;
        JavaPath = configuration.GetSection("CompileServer").GetSection("JavaPath").Value;
    }

    [HttpPost]
    public async Task<ActionResult<RunResult>> Run([FromBody] RunSpecWrapper runSpec) => await Handler.CompileAndRun(runSpec.run_spec);
}