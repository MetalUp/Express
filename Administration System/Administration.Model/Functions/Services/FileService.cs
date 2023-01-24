namespace Model.Functions.Services;

public static class FileService
{
    public static IContext SaveFile(string id, string content, IContext context)
    {
        var file = GetFileById(id, context);
        var file2 = new File(file) { Content = content.AsByteArray() };
        return context.WithUpdated(file, file2);
    }

    public static FileViewModel GetFile(string id, IContext context) =>
        GetFileById(id, context) is { } file ? new FileViewModel(file) : null;

    private static File GetFileById(string id, IContext context)
    {
        return context.Instances<File>().SingleOrDefault(f => f.Id == int.Parse(id));
    }
}