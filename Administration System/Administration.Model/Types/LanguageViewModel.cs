namespace Model.Types;

[ViewModel(typeof(LanguageViewModel_Functions))]
public class LanguageViewModel {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public LanguageViewModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public LanguageViewModel(string alphaName, string compileLanguageId) {
        AlphaName = alphaName;
        CompilerLamguageId = compileLanguageId;
    }

    public string AlphaName { get; set; }

    public string CompilerLamguageId { get; set; }

    public override string ToString() => $"{AlphaName}";
}