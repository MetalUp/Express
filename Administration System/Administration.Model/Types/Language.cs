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
        Version = cloneFrom.Version;
        FileExtension = cloneFrom.FileExtension;
        MIMEType = cloneFrom.MIMEType;
        WrapperFileId = cloneFrom.WrapperFileId;
        WrapperFile = cloneFrom.WrapperFile;
        HelpersFileId = cloneFrom.HelpersFileId;
        HelpersFile = cloneFrom.HelpersFile;
        RegExRulesFileId = cloneFrom.RegExRulesFileId;
        RegExRulesFile = cloneFrom.RegExRulesFile;
    }

    [Hidden]
    public string LanguageID { get; init; }  //      Python = 0, CSharp = 1, VB = 2, Java = 3

    [MemberOrder(10)]
    public string Name { get; init; }

    [MemberOrder(15)]
    public string AlphaName { get; init; }  // e.g. 'csharp'

    [MemberOrder(20)]
    public string Version { get; init; }

    [MemberOrder(30)]
    public string FileExtension { get; init; } //includes the .

    [MemberOrder(40)]
    public string MIMEType { get; init; }  //Most will be text/plain

    #region Wrapper
    [HideInClient]
    public string Wrapper => Encoding.Default.GetString(WrapperFile.Content);

    [Hidden]
    public int? WrapperFileId { get; init; }

    [MemberOrder(40)]
    public virtual File WrapperFile { get; init; }

    #endregion

    #region Helpers

    [HideInClient]
    public string Helpers => Encoding.Default.GetString(HelpersFile.Content);

    [Hidden]
    public int? HelpersFileId { get; init; }

    [MemberOrder(60)]
    public virtual File HelpersFile { get; init; }

    #endregion

    #region RegExRules

    [HideInClient]
    public string RegExRules => Encoding.Default.GetString(RegExRulesFile.Content);

    [Hidden]
    public int? RegExRulesFileId { get; init; }

    [MemberOrder(70)]
    public virtual File RegExRulesFile { get; init; }

    #endregion

    public override string ToString() => $"{Name} {Version}";
}