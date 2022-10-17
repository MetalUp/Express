using Model.Functions.Services;

namespace Model.Functions;

public static class RunSpec_Functions {
    public static string[] DeriveKeys(this RunSpec target) => new[] { target.LanguageID, target.Sourcecode };
    public static RunSpec CreateFromKeys(string[] keys) => new() { LanguageID = keys[0], Sourcecode = keys[1] };
}