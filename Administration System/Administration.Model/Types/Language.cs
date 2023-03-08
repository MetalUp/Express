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
        Version = cloneFrom.Version;
        FileExtension = cloneFrom.FileExtension;
        WrapperFileId = cloneFrom.WrapperFileId;
        WrapperFile = cloneFrom.WrapperFile;
        HelpersFileId = cloneFrom.HelpersFileId;
        HelpersFile = cloneFrom.HelpersFile;
        RegExRulesFileId = cloneFrom.RegExRulesFileId;
        RegExRulesFile = cloneFrom.RegExRulesFile;
        DefaultHiddenCode = cloneFrom.DefaultHiddenCode;
        CompileArguments = cloneFrom.CompileArguments;
    }

    [Hidden]
    public string LanguageID { get; init; }  //      Python = 0, CSharp = 1, VB = 2, Java = 3

    [MemberOrder(10)]
    public string Name { get; init; }

    [MemberOrder(15)]
    public string CSSstyle { get; init; }  // e.g. 'csharp'

    [MemberOrder(20)]
    public string Version { get; init; }

    [MemberOrder(30)]
    public string FileExtension { get; init; } //includes the .

    [MemberOrder(40)]
    public string MIMEType => "text/plain"; 

    #region Wrapper
    [Hidden]
    public string Wrapper => WrapperFile?.ContentsAsString();

    [Hidden]
    public int? WrapperFileId { get; init; }

    [MemberOrder(40)]
    public virtual File WrapperFile { get; init; }

    #endregion

    #region Helpers

    [Hidden]
    public string Helpers => HelpersFile?.ContentsAsString();

    [Hidden]
    public int? HelpersFileId { get; init; }

    [MemberOrder(60)]
    public virtual File HelpersFile { get; init; }

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

    //Depending on language, returns empty string (Python) or empty class HiddenCode (C#, VB) so that tasks with no hidden code still compile. 
    public string DefaultHiddenCode { get; init; }

    public override string ToString() => $"{Name}";
}