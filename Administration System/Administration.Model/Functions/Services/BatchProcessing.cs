using System.IO;
using NakedFramework.Value;
using File = Model.Types.File;
using IOFile = System.IO.File;

namespace Model.Functions.Services;

public static class BatchProcessing {
    public static IContext UpdateFiles(IContext context) {
        var currentDir = Directory.GetCurrentDirectory();
        var dirs = currentDir.Split('\\');
        var i = Array.IndexOf(dirs, "Administration System");
        var filesPath = $"{string.Join('\\', dirs[..i])}\\CommonFiles";

        var csPath = $"{filesPath}\\Common_Files_CS";
        var csFile = context.Instances<File>().Single(f => f.UniqueRef == new Guid("4ec89c87-9dc5-4e0c-bd6d-49473a3f827c"));

        var stream = IOFile.OpenRead($"{csPath}\\Wrapper - File content.txt");

        var fa = new FileAttachment(stream);

        return csFile.ReloadFromExternalFile(fa, context);
    }
}