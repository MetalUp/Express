using Microsoft.Extensions.Logging;

namespace Model.Services;

public static class BatchProcessing {
    public static IContext UpdateFiles(IContext context) =>
        CommonFileDefinitions.defs.Aggregate(context, (c, cfd) => UpdateOrCreateNew(cfd, c));

    private static IContext UpdateOrCreateNew(File def, IContext context) {
        var logger = context.GetService<ILogger<File>>();
        try {
            var f = PersistedFileIfExists(def, context);
            return f == null ? context.WithNew(def) : context.WithUpdated(f, new File(def) { Id = f.Id });
        }
        catch (Exception ex) {
            logger.LogError(ex, $"Error on file: {def.Name}");
            throw;
        }
    }

    private static File PersistedFileIfExists(File def, IContext context) {
        var logger = context.GetService<ILogger<File>>();
        var file = context.Instances<File>().SingleOrDefault(f => f.UniqueRef == def.UniqueRef);
        logger.LogWarning(file is null ? $"Failed to find {def.Name}" : $"Found {def.Name}");

        return file;
    }
}