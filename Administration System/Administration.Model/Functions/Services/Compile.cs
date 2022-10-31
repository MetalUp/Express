using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Model.Functions.Services;

public static class Compile {
    private const string CompileServerName = "CompileServer";
    private const string AdminServerName = "AdminServer";
    private static readonly HttpClient Client = new();
    private static string compileServer;

    static Compile() => compileServer = Environment.GetEnvironmentVariable(CompileServerName);

    private static void CheckServer(IContext context) {
        if (compileServer is null) {
            var configuration = context.GetService<IConfiguration>();
            compileServer = configuration.GetSection(AdminServerName).GetSection(CompileServerName).Value;
        }
    }

    private static HttpRequestMessage CreateMessage(IContext context, HttpMethod method, string path, HttpContent content = null) {
        CheckServer(context);
        var request = new HttpRequestMessage(method, path);
        var httpContext = context.GetService<IHttpContextAccessor>().HttpContext;
        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        if (token is not null) {
            request.Headers.Add("Authorization", token);
        }
        if (content is not null) {
            request.Content = content;
        }

        return request;
    }

    public static IQueryable<Language> Languages(IContext context) {
        var httpRequest = CreateMessage(context, HttpMethod.Get, $"{compileServer}/languages");

        using var response = Client.Send(httpRequest);

        if (response.IsSuccessStatusCode) {
            var languages = ReadAs<List<string[]>>(response);
            return languages.Select(l => new Language { LanguageID = l[0], Version = l[1] }).AsQueryable();
        }

        throw new HttpRequestException("compile server request failed", null, response.StatusCode);
    }

    private static (RunResult, IContext) Execute(string languageID, string code, string url, IContext context) {
        using var content = JsonContent.Create(RunSpec.FromParams(languageID, code), new MediaTypeHeaderValue("application/json"));
        var request = CreateMessage(context, HttpMethod.Post, url, content);

        using var response = Client.SendAsync(request).Result;

        if (response.IsSuccessStatusCode) {
            var apiRunResult = ReadAs<ApiRunResult>(response);
            return (apiRunResult?.ToRunResult(), context);
        }

        throw new HttpRequestException("compile server request failed", null, response.StatusCode);
    }

  

    public static (RunResult, IContext) Runs(string languageID, string code, IContext context) => Execute(languageID, code, $"{compileServer}/runs", context);

    public static (RunResult, IContext) Tests(string languageID, string code, IContext context) => Execute(languageID, code, $"{compileServer}/tests", context);

    // new placeholder functions 

    private static (RunResult, IContext) Execute((string languageId, string code) wrapped, string url, IContext context) {
        var (languageId, code) = wrapped;
        using var content = JsonContent.Create(RunSpec.FromParams(languageId, code), new MediaTypeHeaderValue("application/json"));
        var request = CreateMessage(context, HttpMethod.Post, url, content);

        using var response = Client.SendAsync(request).Result;

        if (response.IsSuccessStatusCode) {
            var apiRunResult = ReadAs<ApiRunResult>(response);
            return (apiRunResult?.ToRunResult(), context);
        }

        throw new HttpRequestException("compile server request failed", null, response.StatusCode);
    }

    private static (string, string) WrapCode(int taskId, string code, string expression, IContext context) {
        var task = context.Instances<Task>().Single(t => t.Id == taskId);
        var language = task.Language.ToString();
        // todo
        return (language, code);
    }

    public static (RunResult, IContext) EvaluateExpression(int taskId, string expression, string code, IContext context) => Execute(WrapCode(taskId, code, expression, context), $"{compileServer}/runs", context);

    public static (RunResult, IContext) SubmitCode(int taskId, string languageID, string code, IContext context) => Execute(WrapCode(taskId, code, "", context), $"{compileServer}/runs", context);

    public static (RunResult, IContext) RunTests(int taskId, string languageID, string code, IContext context) => Execute(WrapCode(taskId, code, "", context), $"{compileServer}/tests", context);

    private static T ReadAs<T>(HttpResponseMessage response) {
        using var sr = new StreamReader(response.Content.ReadAsStream());
        var json = sr.ReadToEnd();
        return JsonSerializer.Deserialize<T>(json);
    }

    private class RunSpec {
        public InnerSpec run_spec { get; init; }
        public static RunSpec FromParams(string languageID, string code) => new() { run_spec = new InnerSpec { language_id = languageID, sourcecode = code } };

        public class InnerSpec {
            public string language_id { get; init; }
            public string sourcecode { get; init; }
        }
    }

    private class ApiRunResult {
        public string cmpinfo { get; set; }
        public int outcome { get; set; }
        public string run_id { get; set; }
        public string stderr { get; set; }
        public string stdout { get; set; }

        public RunResult ToRunResult() => new() { Cmpinfo = cmpinfo, Outcome = outcome, RunID = run_id, Stderr = stderr, Stdout = stdout };
    }
}