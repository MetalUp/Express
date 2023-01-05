using NakedFramework.Value;
using System.Text;

namespace Model.Functions
{
    public static class Hint_Functions
    {
        [Edit]
        public static IContext EditName(
             this Hint hint,
             string name,
             IContext context) =>
                 context.WithUpdated(hint, new(hint) { Name = name });

        [Edit]
        public static IContext EditNumber(
             this Hint hint,
             int number,
             IContext context) =>
         context.WithUpdated(hint, new(hint) { Number = number });


        [Edit]
        public static IContext EditCostInMarks(
            this Hint hint,
            int costInMarks,
            IContext context) =>
        context.WithUpdated(hint, new(hint) { CostInMarks = costInMarks });

        public static IContext ReloadContentFromExternalFile(this Hint hint, FileAttachment externalFile, IContext context) =>
            context.WithUpdated(hint, new Hint(hint) { Name = externalFile.Name, Content = externalFile.GetResourceAsByteArray() });

        public static IContext EditContentAsString(this Hint hint, [MultiLine(20)] string content, IContext context) =>
            context.WithUpdated(hint, new Hint(hint) { Content = content.AsByteArray() });

        public static string Default1EditContentAsString(this Hint hint) =>
            hint.ContentsAsString();

        internal static string ContentsAsString(this Hint hint) => hint.Content.AsASCIIonly();
    }

}
