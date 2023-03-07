namespace Model.Types;

[ViewModel(typeof(FileViewModel_Functions))]
public class FileViewModel
{
    public FileViewModel() { }

    public FileViewModel(File file)
    {
        FileId = file.Id;
        Content = file.ContentsAsString() ?? "";
        LanguageAlphaName = file.Language?.AlphaName ?? "";
        Name = file.Name;
    }

    internal int FileId { get; init; }

    public string Content { get; init; }

    public string Mime { get; init; }

    public string LanguageAlphaName { get; init; }

    public string Name { get; init; }
}