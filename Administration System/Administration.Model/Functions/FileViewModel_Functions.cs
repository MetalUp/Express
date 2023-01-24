namespace Model.Functions;

public static class FileViewModel_Functions {
    public static string[] DeriveKeys(this FileViewModel target) =>
        new[] { target.FileId.ToString() };

    public static FileViewModel CreateFromKeys(string[] keys, IContext context) {
        var file = context.Instances<File>().Single(f => f.Id == int.Parse(keys[0]));
        return new FileViewModel(file);
    }
}