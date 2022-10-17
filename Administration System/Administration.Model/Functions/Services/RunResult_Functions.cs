using Model.Functions.Services;

namespace Model.Functions;

public static class RunResult_Functions {
    public static string[] DeriveKeys(this RunResult target) => new[] { "1" };
    public static RunResult CreateFromKeys(string[] keys) => new() { };
}