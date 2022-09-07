namespace Model.Functions
{

    [Named("Organisations")]
    public static class Organisations_Menu
    {
        public static (Organisation, IContext) CreateNewOrganisation(string name, IContext context)
        {
            var o = new Organisation {Name = name };
            return (o, context.WithNew(o));
        }

        public static IQueryable<Organisation> AllOrganisations(IContext context) => context.Instances<Organisation>();
    }

}