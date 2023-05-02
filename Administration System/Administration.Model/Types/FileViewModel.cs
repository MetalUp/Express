namespace Model.Types;

[ViewModel(typeof(FileViewModel_Functions))]
public class FileViewModel
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public FileViewModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public FileViewModel(File file)
    {
        FileId = file.Id;
        Content = file.ContentsAsString() ?? "";
        LanguageAlphaName = file.Language?.CSSstyle ?? "";
        Name = file.ToString();
        Mime = file.MIMEType();
    }

    internal int FileId { get; init; }

    public string Content { get; init; }

    public string Mime { get; init; }

    public string LanguageAlphaName { get; init; }

    public string Name { get; init; }
}