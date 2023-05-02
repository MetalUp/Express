namespace Model.Types;

[ViewModel(typeof(LanguageViewModel_Functions))]
public class LanguageViewModel {
    public LanguageViewModel() { }

    public LanguageViewModel(string alphaName, string compileLanguageId) {
        AlphaName = alphaName;
        CompilerLamguageId = compileLanguageId;
    }

    public string AlphaName { get; set; }

    public string CompilerLamguageId { get; set; }

    public override string ToString() => $"{AlphaName}";
}