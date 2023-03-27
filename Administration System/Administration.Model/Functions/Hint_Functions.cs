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

        internal static string ContentsAsString(this Hint hint) => hint.File.Content.AsASCIIonly();
    }
}
