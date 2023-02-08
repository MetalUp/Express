namespace Model.Types;

[ViewModel(typeof(LanguageViewModel_Functions))]
public class LanguageViewModel {
    public LanguageViewModel() { }

    public LanguageViewModel(string alphaName, string version) {
        AlphaName = alphaName;
        Version = version;
    }

    public string AlphaName { get; set; }

    public string Version { get; set; }

    private string DisplayVersion => string.IsNullOrEmpty(Version) ? string.Empty : $"({Version})";

    public override string ToString() => $"{AlphaName}{DisplayVersion}";
}