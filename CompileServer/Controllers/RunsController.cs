using CompileServer.Models;
using CompileServer.Workers;
using Microsoft.AspNetCore.Mvc;

namespace CompileServer.Controllers;

[Route("restapi/[controller]")]
public class RunsController : ControllerBase {
    private readonly ILogger<RunsController> logger;
    public static string PythonPath { get; private set; } = "";
    public static string JavaPath { get; private set; } = "";

    private const string PythonPathName = "PythonPath";
    private const string JavaPathName = "JavaPath";
    private const string CompileServerName = "CompileServer";

    public RunsController(ILogger<RunsController> logger, IConfiguration configuration) {
        this.logger = logger;

        PythonPath = Environment.GetEnvironmentVariable(PythonPathName) ?? configuration.GetSection(CompileServerName).GetSection(PythonPathName).Value;
        JavaPath = Environment.GetEnvironmentVariable(JavaPathName) ?? configuration.GetSection(CompileServerName).GetSection(JavaPathName).Value;
    }

    [HttpPost]
    public async Task<ActionResult<RunResult>> Run([FromBody] RunSpecWrapper runSpec) => await Handler.CompileAndRun(runSpec.run_spec);
}