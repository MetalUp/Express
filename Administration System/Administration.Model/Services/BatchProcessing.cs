namespace Model.Functions.Services;

public static class BatchProcessing
{
    public static IContext UpdateFiles(IContext context) =>
        CommonFileDefinitions.defs.Aggregate(context, (c, cfd) => UpdateOrCreateNew(cfd, c));

    static IContext UpdateOrCreateNew(File def, IContext context)
    {
        File f = PersistedFileIfExists(def, context);
        return f == null ?
            context.WithNew(def) :
            context.WithUpdated(f, new File(def) { Id = f.Id });
    }

    static File PersistedFileIfExists(File def, IContext context) =>
       context.Instances<File>().SingleOrDefault(f => f.UniqueRef == def.UniqueRef);
}