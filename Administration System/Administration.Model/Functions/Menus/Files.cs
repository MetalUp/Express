using NakedFramework.Value;
using System.Text;

namespace Model.Functions.Menus
{
    public static class Files
    {
        [MemberOrder(10)]
        public static IQueryable<File> AllFiles(IContext context) => context.Instances<File>();

        [MemberOrder(20)]
        [RenderEagerly]
        [TableView(false,nameof(File.Name), nameof(File.ContentType), nameof(File.Language))]
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
        [RenderEagerly]
        [TableView(false, nameof(File.Name), nameof(File.ContentType), nameof(File.Language))]
        public static IQueryable<File> FindFiles(string name, IContext context) =>
            context.Instances<File>().Where(f => f.Name.ToUpper().Contains(name.ToUpper()));

        [MemberOrder(50)]
        [CreateNew]
        public static (File, IContext) CreateNewFileFromExtFile(
            FileAttachment extFile, 
            ContentType type,
            [Optionally] Language language,
            IContext context) =>
                CreateNewFile(extFile.Name, type, language, extFile.GetResourceAsByteArray(), context);

        [MemberOrder(60)]
        [CreateNew]
        public static (File, IContext) CreateNewFileAsString(
            string name, ContentType type,  
            [Optionally] Language language, 
            [Optionally] [MultiLine(20)] string content, 
            IContext context) =>
                CreateNewFile(name, type,language, content.AsByteArray(), context);

        internal static (File, IContext) CreateNewFile(string name, ContentType type, Language language, byte[] content, IContext context)
        {
            var me = Users.Me(context);
            var f = new File()
            {
                Name = name,
                Content = content,
                ContentType = type,
                Language = language,
                AuthorId = me.Id,
                Author = me
            };
            return (f, context.WithNew(f));
        }

        [MemberOrder(70)]
        [RenderEagerly]
        [TableView(false, nameof(File.Name), nameof(File.ContentType), nameof(File.Language), nameof(File.UniqueRef))]
        public static IQueryable<File> FilesWithUniqueRef(IContext context) =>
            context.Instances<File>().Where(f => f.UniqueRef != null);
    }
}
