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

        internal static string ContentsAsString(this Hint hint) => hint.File.Content.AsASCIIonly();
    }
}
