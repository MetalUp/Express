using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Model.Functions.Services;

public static class Compile {
    private static readonly HttpClient Client = new();
    private static readonly string CompileServer = "https://metalupcompileserver.azurewebsites.net/restapi";

    public static IQueryable<Language> Languages() {
        var httpRequest = new HttpRequestMessage(new HttpMethod("GET"), $"{CompileServer}/languages");

        using var response = Client.Send(httpRequest);

        if (response.IsSuccessStatusCode) {
            using var sr = new StreamReader(response.Content.ReadAsStream());

            var json = sr.ReadToEnd();
            var source = JsonSerializer.Deserialize<List<string[]>>(json);

            return source.Select(l => new Language { LanguageID = l[0], Version = l[1] }).AsQueryable();
        }

        throw new HttpRequestException("compile server request failed", null, response.StatusCode);
    }

    public static (RunResult, IContext) Runs(string languageID, string code, IContext context) {
        var runSpec = RunSpec.FromParams(languageID, code);
        using var content = JsonContent.Create(runSpec, new MediaTypeHeaderValue("application/json"));
        using var response = Client.PostAsync($"{CompileServer}/runs", content).Result;

        if (response.IsSuccessStatusCode) {
            using var sr = new StreamReader(response.Content.ReadAsStream());
            var json = sr.ReadToEnd();
            var apiRunResult = JsonSerializer.Deserialize<ApiRunResult>(json);

            return (apiRunResult?.ToRunResult(), context);
        }

        throw new HttpRequestException("compile server request failed", null, response.StatusCode);
    }

    private class RunSpec {
        public class InnerSpec {
            public string language_id { get; init; }
            public string sourcecode { get; init; }
        }
        public InnerSpec run_spec { get; init; }
        public static RunSpec FromParams(string languageID, string code) => new() { run_spec = new InnerSpec { language_id = languageID, sourcecode = code } };
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