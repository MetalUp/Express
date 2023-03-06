using NakedFramework.Value;
using System.Text;

namespace Model.Functions.Menus
{
    public static class Files
    {
        [MemberOrder(10)]
        public static IQueryable<File> AllFiles(IContext context) => context.Instances<File>();

        [MemberOrder(20)]
        public static IQueryable<File> ListFiles([Optionally] Language? language, [Optionally] ContentType? contentType, IContext context)
        {
            string langId = language?.LanguageID;
            return context.Instances<File>().Where(f => (langId == null || f.LanguageId == langId) && (contentType == null || f.ContentType == contentType))
                .OrderBy(f => f.Name);
        }

        [MemberOrder(30)]
        public static File FindFile(File fileName, IContext context) => fileName;

        public static IQueryable<File> AutoComplete0FindFile(string partialName, IContext context) =>
    context.Instances<File>().Where(f => f.Name.ToUpper().Contains(partialName.ToUpper()));

        [MemberOrder(40)]
        public static IQueryable<File> FindFiles(string name, IContext context) =>
            context.Instances<File>().Where(f => f.Name.ToUpper().Contains(name.ToUpper()));

        [MemberOrder(50)]
        [CreateNew]
        public static (File, IContext) CreateNewFileFromExtFile(FileAttachment extFile, ContentType type, IContext context) =>
            CreateNewFile(extFile.Name, extFile.MimeType,type, extFile.GetResourceAsByteArray(), context);

        [MemberOrder(60)]
        [CreateNew]
        public static (File, IContext) CreateNewFileAsString(string name, string mimeType, ContentType type, [MultiLine(20)] string content, IContext context) =>
            CreateNewFile(name, mimeType, type, content.AsByteArray(), context);

        public static List<string> Choices1CreateNewFileAsString() => new List<string> { "text/plain", "text/html", "application/json" };

        private static (File, IContext) CreateNewFile(string name, string mimeType, ContentType type, byte[] content, IContext context)
        {
            var me = Users.Me(context);
            var f = new File()
            {
                Name = name,
                Content = content,
                Mime = mimeType,
                ContentType = type,
                AuthorId = me.Id,
                Author = me
            };
            return (f, context.WithNew(f));
        }
    }
}
