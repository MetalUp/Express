namespace Model.Menus
{
    public static class Languages
    {

        public static IQueryable<Language> AllLanguages(IContext context) => context.Instances<Language>();

        internal static Language FindLanguageByName(string name, IContext context) =>
            context.Instances<Language>().SingleOrDefault(l => l.Name == name);

        [CreateNew]
        public static (Language, IContext) CreateNewLanguage(string name, IContext context)
        {
            var l = new Language()
            {
                Name = name
            };
            return (l, context.WithNew(l));
        }

    }
}
