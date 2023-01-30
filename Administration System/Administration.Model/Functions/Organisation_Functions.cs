
namespace Model.Functions
{
    public static class Organisation_Functions
    {
        #region Editing
        [Edit]
        public static IContext EditName(
            this Organisation org,
            string name,
            IContext context) =>
            context.WithUpdated(org, new(org) { Name = name });

        [Edit]
        public static IContext EditDetails(
            this Organisation org,
            string details,
            IContext context) =>
            context.WithUpdated(org, new(org) { Details = details });
        #endregion

    }
}
