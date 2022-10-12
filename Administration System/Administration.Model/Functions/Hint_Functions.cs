using NakedFramework.Value;

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

        public static IContext SpecifyHtmlFile(
                this Hint hint,
                FileAttachment file,
                IContext context) =>
                    context.WithUpdated(hint,
                        new Hint(hint)
                        {
                            FileContent = file.GetResourceAsByteArray(),
                        });
    }

}
