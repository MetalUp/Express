namespace Model.Functions;

public static class LanguageViewModel_Functions {
    public static string[] DeriveKeys(this LanguageViewModel target) => new[] { target.AlphaName, target.CompilerLamguageId };

    public static LanguageViewModel CreateFromKeys(string[] keys, IContext context) => new(keys[0], keys[1]);
}