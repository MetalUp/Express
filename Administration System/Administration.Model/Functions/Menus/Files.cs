using NakedFramework.Value;
using System.Text;

namespace Model.Functions.Menus
{
    public static class Files
    {
        public static IQueryable<File> AllFiles(IContext context) => context.Instances<File>();

        public static IQueryable<File> ListFiles([Optionally] Language? language, [Optionally] ContentType? contentType, IContext context)
        {
            string langId = language?.LanguageID;
            return context.Instances<File>().Where(f => (langId == null || f.LanguageId == langId) && (contentType == null || f.ContentType == contentType));
        }
        public static File FindFile(File fileName, IContext context) => fileName;

        public static IQueryable<File> AutoComplete0FindFile(string partialName, IContext context) =>
            context.Instances<File>().Where(f => f.Name.ToUpper().Contains(partialName.ToUpper()));

        [CreateNew]
        public static (File, IContext) CreateNewFileFromExtFile(FileAttachment extFile, IContext context) =>
            CreateNewFile(extFile.Name, extFile.MimeType, extFile.GetResourceAsByteArray(), context);

        [CreateNew]
        public static (File, IContext) CreateNewFileAsString(string name, string mimeType, [MultiLine(20)] string content, IContext context) =>
            CreateNewFile(name, mimeType, content.AsByteArray(), context);

        public static List<string> Choices1CreateNewFileAsString() => new List<string> { "text/plain", "text/html", "application/json" };

        private static (File, IContext) CreateNewFile(string name, string mimeType, byte[] content, IContext context)
        {
            var me = Users.Me(context);
            var f = new File()
            {
                Name = name,
                Content = content,
                Mime = mimeType,
                AuthorId = me.Id,
                Author = me
            };
            return (f, context.WithNew(f));
        }


    }
}
