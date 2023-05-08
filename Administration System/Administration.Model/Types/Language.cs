using NakedFramework.Value;
using System.Text;

namespace Model.Types;

[Bounded]
public class Language
{
    public Language() { }

    public Language(Language cloneFrom)
    {
        LanguageID = cloneFrom.LanguageID;  
        Name = cloneFrom.Name;
        CSSstyle = cloneFrom.CSSstyle;
        CompilerLanguageId = cloneFrom.CompilerLanguageId;
        WrapperFileId = cloneFrom.WrapperFileId;
        WrapperFile = cloneFrom.WrapperFile;
        RegExRulesFileId = cloneFrom.RegExRulesFileId;
        RegExRulesFile = cloneFrom.RegExRulesFile;
        CompileArguments = cloneFrom.CompileArguments;
    }

    [Hidden]
    public string LanguageID { get; init; }  //      Python = 0, CSharp = 1, VB = 2, Java = 3

    [MemberOrder(10)]
    public string Name { get; init; }

    [MemberOrder(15)]
    [Named("CSS Style")]
    public string CSSstyle { get; init; }  // e.g. 'csharp'

    [MemberOrder(20)]
    public string CompilerLanguageId { get; init; }

    #region Wrapper
    [Hidden]
    public string Wrapper => WrapperFile?.ContentsAsString();

    [Hidden]
    public int? WrapperFileId { get; init; }

    [MemberOrder(40)]
    public virtual File WrapperFile { get; init; }

    #endregion

    #region RegExRules
    [Hidden]
    public string RegExRules => RegExRulesFile?.ContentsAsString();

    [Hidden]
    public int? RegExRulesFileId { get; init; }

    [MemberOrder(70)]
    public virtual File RegExRulesFile { get; init; }
    #endregion

    [MemberOrder(80)]
    public string CompileArguments { get; init; }

    public override string ToString() => $"{Name}";
}