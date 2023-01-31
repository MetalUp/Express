namespace Model.Functions.Menus
{
    public static class Languages
    {

        public static IQueryable<Language> AllLanguages(IContext context) => context.Instances<Language>();

        internal static Language FindLanguageByName(string name, IContext context) =>
            context.Instances<Language>().SingleOrDefault(l => l.Name == name);

    }
}
