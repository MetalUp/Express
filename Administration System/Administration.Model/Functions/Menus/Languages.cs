namespace Model.Functions.Menus
{
    public static class Languages
    {

        public static IQueryable<Language> AllLanguages(IContext context) => context.Instances<Language>();
    }
}
