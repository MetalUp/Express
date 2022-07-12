namespace Model.Functions
{

    [Named("Organisations")]
    public static class Organisation_MenuFunctions
    {
        public static (Organisation, IContext) CreateNewInvitation(string name, IContext context)
        {
            var o = new Organisation {Name = name };
            return (o, context.WithNew(o));
        }

        public static IQueryable<Organisation> AllOrganisations(IContext context) => context.Instances<Organisation>();

        public static Organisation MyOrganisation(IContext context) =>
            User_MenuFunctions.Me(context).Organisation;
    }

    public static class Organisations_Functions
    {

    }
}