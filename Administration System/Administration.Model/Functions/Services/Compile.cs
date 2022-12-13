using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Model.Functions.Services;

public static class Compile
{
    private const string CompileServerName = "CompileServer";
    private const string AdminServerName = "AdminServer";
    private static readonly HttpClient Client = new();
    private static string compileServer;

    static Compile() => compileServer = Environment.GetEnvironmentVariable(CompileServerName);

    private static void CheckServer(IContext context)
    {
        if (compileServer is null)
        {
            var configuration = context.GetService<IConfiguration>();
            compileServer = configuration.GetSection(AdminServerName).GetSection(CompileServerName).Value;
        }
    }

    private static HttpRequestMessage CreateMessage(IContext context, HttpMethod method, string path, HttpContent content = null)
    {
        CheckServer(context);
        var request = new HttpRequestMessage(method, path);
        var httpContext = context.GetService<IHttpContextAccessor>().HttpContext;
        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (token is not null)
        {
            request.Headers.Add("Authorization", token);
        }

        if (content is not null)
        {
            request.Content = content;
        }

        return request;
    }

    private static (RunResult, IContext) Execute((string languageId, string code) wrapped, string url, IContext context)
    {
        var (languageId, code) = wrapped;
        using var content = JsonContent.Create(RunSpec.FromParams(languageId, code), new MediaTypeHeaderValue("application/json"));
        var request = CreateMessage(context, HttpMethod.Post, url, content);

        using var response = Client.SendAsync(request).Result;

        if (response.IsSuccessStatusCode)
        {
            var apiRunResult = ReadAs<ApiRunResult>(response);
            return (apiRunResult?.ToRunResult(), context);
        }

        throw new HttpRequestException("compile server request failed", null, response.StatusCode);
    }

    private static (string, string) WrapCode(IContext context, int taskId, string code, bool includeTests, string expression = null)
    {
        var task = context.Instances<Task>().Single(t => t.Id == taskId);
        var language = task.Language;
        var testCode = includeTests ? task.Tests : "";

        var wrappedCode = task.Wrapper
                                  .Replace("<Expression>", expression ?? "\"\"")
                                  .Replace("<StudentCode>", code)
                                  .Replace("<HiddenCode>", task.HiddenCode)
                                  .Replace("<Helpers>", task.Helpers)
                                  .Replace("<Tests>", testCode);

        return (language, wrappedCode);
    }

    public static (RunResult, IContext) EvaluateExpression(int taskId, string expression, [Optionally] string code, IContext context) =>
        Execute(WrapCode(context, taskId, code, false, expression), $"{compileServer}/runs", context);

    public static (RunResult, IContext) SubmitCode(int taskId, string code, IContext context)
    {
        (var result, var context2) = Execute(WrapCode(context, taskId, code, false), $"{compileServer}/compiles", context);
        if (result.Outcome == (int)CompilerOutcome.CompilationError)
        {
            return (result, Activities.SubmitCodeFail(taskId, code, result.Cmpinfo, context));
        }
        else if (result.Outcome == (int)CompilerOutcome.Ok)
        {
            return (result, Activities.SubmitCodeSuccess(taskId, code, context));
        }
        return (result, context2);
    }

    public static (RunResult, IContext) RunTests(int taskId, string code, IContext context)
    {
        (var result, var context2) = Execute(WrapCode(context, taskId, code, true), $"{compileServer}/tests", context);
        if (result.Outcome == (int)CompilerOutcome.Ok)
        {
            //TODO: Temporary solution, pending moving RegEx rules server side.
            if (result.Stdout.Contains("Failed!") || result.Stdout.Contains("FAIL")) // C#/VB and Python, respectively {
            {
               return (result, Activities.RunTestsFail(taskId, result.Stdout, code, context2));
            }
            else
            {
                return (result, Activities.RunTestsSuccess(taskId, code, context2));
            }
        }
        return (result, context2);
    }

    private static T ReadAs<T>(HttpResponseMessage response)
    {
        using var sr = new StreamReader(response.Content.ReadAsStream());
        var json = sr.ReadToEnd();
        return JsonSerializer.Deserialize<T>(json);
    }

    private class RunSpec
    {
        public InnerSpec run_spec { get; init; }
        public static RunSpec FromParams(string languageID, string code) => new() { run_spec = new InnerSpec { language_id = languageID, sourcecode = code } };

        public class InnerSpec
        {
            public string language_id { get; init; }
            public string sourcecode { get; init; }
        }
    }

    private class ApiRunResult
    {
        public string cmpinfo { get; set; }
        public int outcome { get; set; }
        public string run_id { get; set; }
        public string stderr { get; set; }
        public string stdout { get; set; }

        public RunResult ToRunResult() => new() { Cmpinfo = cmpinfo, Outcome = outcome, RunID = run_id, Stderr = stderr, Stdout = stdout };
    }
}