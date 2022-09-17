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
        context.WithUpdated(hint, new(hint) { Title = title });

        [Edit]
        public static IContext EditCostInMarks(
            this Hint hint,
            int costInMarks,
            IContext context) =>
        context.WithUpdated(hint, new(hint) { CostInMarks = costInMarks });

        public static FileAttachment ViewFile() => throw new NotImplementedException();

        public static IContext UploadFile(this Hint hint, FileAttachment file, IContext context) =>
            throw new NotImplementedException();
        //TODO: delegate to file service
    }

}
