namespace Model.Functions.Menus
{
    public static class Organisations
    {
        public static Organisation MyOrganisation(IContext context) =>
            Users.Me(context).Organisation;

        public static IQueryable<Organisation> AllOrganisations(IContext context) => context.Instances<Organisation>();

        public static IQueryable<Organisation> FindOrganisation(string nameMatch, IContext context) => 
            context.Instances<Organisation>().Where(org => org.Name.ToUpper().Contains(nameMatch.ToUpper()));

        public static (Organisation, IContext) CreateNewOrganisation(string name, IContext context)
        {
            var org = new Organisation() { Name = name };
            return (org, context.WithNew(org));
        }
    }
}
