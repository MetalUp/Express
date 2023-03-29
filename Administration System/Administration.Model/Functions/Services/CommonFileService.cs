namespace Model.Functions.Services
{
    public class CommonFileService
    {
         public const string Python = "1";
        //etc

        //Common File Definitions ('cdfs')
        static List<File> cfds = new List<File>
        {
            CreateNewCfd("Functions", ContentType.Wrapper, Python, @"C:/jfhskdjfhj", new Guid("ab42-...")),
            CreateNewCfd("Wrapper", ContentType.Wrapper, Python, @"C:/xfgdf", new Guid( "12e3-..."))
        };

        private static File CreateNewCfd(string name, ContentType contentType, string language, string pathToFile, Guid guid)
        {
            //byte[] content = get content from path & read in as byte[]
            throw new NotImplementedException();
        }

        static File GetCorrespondingFileFromDatabaseIfExists(File cdf, IContext context) =>
            context.Instances<File>().SingleOrDefault(f => f.UniqueRef == cdf.UniqueRef);

        static IContext UpdateOrCreateNew(File cfd, IContext context)
        {
            File f = GetCorrespondingFileFromDatabaseIfExists(cfd, context);
            return f == null ? 
                context.WithNew(cfd) :
                context.WithUpdated(f, new File(cfd) { Id = f.Id });
        }

        public static IContext UpdateAllCommonFiles(IContext context) =>
            cfds.Aggregate(context, (c, cfd) => UpdateOrCreateNew(cfd, c));

    }
}
