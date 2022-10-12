﻿using NakedFramework.Value;
using System.Text;

namespace Model.Functions
{
    public static class File_Functions
    {
        public static IContext ReloadFromExternalFile(this File file, FileAttachment externalFile, IContext context) =>
            context.WithUpdated(file, new File(file) { Content = externalFile.GetResourceAsByteArray(), Name=externalFile.Name, Mime = externalFile.MimeType });

        public static IContext EditContentAsString(this File file, [MultiLine(20)] string content, IContext context) =>
            context.WithUpdated(file, new File(file) {Content = Encoding.ASCII.GetBytes(content)});

        public static string Default1EditContentAsString(this File file) =>
            Encoding.Default.GetString(file.Content);

    }
}
