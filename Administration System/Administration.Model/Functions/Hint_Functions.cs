using NakedFramework.Value;
using System.Text;

namespace Model.Functions
{
    public static class Hint_Functions
    {
        [Edit]
        public static IContext EditNumber(
             this Hint hint,
             int number,
             IContext context) =>
         context.WithUpdated(hint, new(hint) { Number = number });

        [Edit]
        public static IContext EditTitle(
            this Hint hint,
            string title,
            IContext context) =>
        context.WithUpdated(hint, new(hint) { Name = title });

        [Edit]
        public static IContext EditCostInMarks(
            this Hint hint,
            int costInMarks,
            IContext context) =>
        context.WithUpdated(hint, new(hint) { CostInMarks = costInMarks });

        public static IContext LoadHtmlFile(
                this Hint hint,
                FileAttachment file,
                IContext context) =>
                    context.WithUpdated(hint, new Hint(hint) { FileContent = file.GetResourceAsByteArray() });

        public static IContext LoadContentAsString(
            this Hint hint,
            string content,
            IContext context) =>
                context.WithUpdated(hint, new Hint(hint) { FileContent = Encoding.ASCII.GetBytes(content) });

        public static string Default1LoadContentAsString(this Hint hint) => Encoding.Default.GetString(hint.FileContent);
    }

}
