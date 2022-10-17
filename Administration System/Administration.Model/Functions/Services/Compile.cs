using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace Model.Functions.Services;

public static class Compile {
    private static readonly HttpClient Client = new();
    private static readonly string compileServer = "https://metalupcompileserver.azurewebsites.net/restapi/languages";

    public static IQueryable<Language> Languages() {
        var httpRequest = new HttpRequestMessage(new HttpMethod("GET"), compileServer);

        using var response = Client.Send(httpRequest);

        if (response.IsSuccessStatusCode) {
            using var sr = new StreamReader(response.Content.ReadAsStream());

            var json = sr.ReadToEnd();
            var source = JsonSerializer.Deserialize<List<string[]>>(json);

            return source.Select(l => new Language { LanguageID = l[0], Version = l[1] }).AsQueryable();
        }

        throw new HttpRequestException("compile server request failed", null, response.StatusCode);
    }
}