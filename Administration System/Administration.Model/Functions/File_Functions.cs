using NakedFramework.Value;
using System.Text;
using System.Text.RegularExpressions;

namespace Model.Functions
{
    public static class File_Functions
    {
        public static IContext ReloadFromExternalFile(this File file, FileAttachment externalFile, IContext context) =>
            context.WithUpdated(file, new File(file) { Content = externalFile.GetResourceAsByteArray(), Name=externalFile.Name, Mime = externalFile.MimeType });

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
    }
}
