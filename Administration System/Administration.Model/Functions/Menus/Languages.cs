namespace Model.Functions.Menus
{
    public class Languages
    {

        public static IQueryable<Language> AllLanguages(IContext context) => context.Instances<Language>();
    }
}
