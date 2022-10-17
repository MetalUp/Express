namespace Model.Functions;

public static class Language_Functions {
    public static string[] DeriveKeys(this Language target) => new[] { target.LanguageID, target.Version };
    public static Language CreateFromKeys(string[] keys) => new() { LanguageID = keys[0], Version = keys[1] };
}