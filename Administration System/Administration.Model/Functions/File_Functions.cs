using NakedFramework.Value;
using System.Text;
using System.Text.RegularExpressions;

namespace Model.Functions
{
    public static class File_Functions
    {
        [MemberOrder(10)]
        public static IContext ReloadFromExternalFile(this File file, FileAttachment externalFile, IContext context) =>
            context.WithUpdated(file, new File(file) { Content = externalFile.GetResourceAsByteArray(), Name=externalFile.Name, Mime = externalFile.MimeType });

        [MemberOrder(20)]
        public static IContext EditContentAsString(this File file, [MultiLine(20)] string content, IContext context) =>
            context.WithUpdated(file, new File(file) {Content =content.AsByteArray()});

        public static string Default1EditContentAsString(this File file) => file.ContentsAsString();

        internal static string ContentsAsString(this File file) => file.Content.AsASCIIonly();

        internal static string AsASCIIonly(this byte[] bytes) =>
            Regex.Replace(Encoding.Default.GetString(bytes), @"[^\u0000-\u007F]+", string.Empty);

        internal static byte[] AsByteArray(this string str) => Encoding.ASCII.GetBytes(str);

        #region Editing
        [Edit]
        public static IContext EditName(
         this File file,
         string name,
         IContext context) =>
             context.WithUpdated(file, new(file) { Name = name });

        [Edit]
        public static IContext EditLanguage(
                 this File file,
                 Language language,
                 IContext context) =>
                     context.WithUpdated(file, new(file) { LanguageId = language.LanguageID, Language = language});

        [Edit]
        public static IContext EditContentType(
            this File file,
            ContentType? contentType,
            IContext context) =>
                context.WithUpdated(file, new(file) { ContentType = contentType });
        #endregion

        #region Find uses
        [MemberOrder(30)]
        public static IQueryable<Project> FindReferencingProjects(this File file, IContext context)
        {
            int id = file.Id;
            return context.Instances<Project>().Where(p => p.CommonTestsFileId == id || p.CommonHiddenCodeFileId == id || p.WrapperFileId == id || p.RegExRulesFileId == id);
        }

        [MemberOrder(40)]
        public static IQueryable<Task> FindReferencingTasks(this File file, IContext context)
        {
            int id = file.Id;
            return context.Instances<Task>().Where(p => p.TestsFileId == id || p.HiddenCodeFileId == id || p.WrapperFileId == id || p.RegExRulesFileId == id);
        }


        #endregion

        internal static string ValidateContentType(this File file, ContentType type) =>
            file.ContentType == type ? null : $"File must have Content Type {type}";
    }

}
