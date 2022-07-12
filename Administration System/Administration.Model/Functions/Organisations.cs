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
    }

    public static class Organisations_Functions
    {
        public static IQueryable<User> AllMyStudents(IContext context)
        {
            int myOrgId = User_MenuFunctions.LoggedOnUser(context).OrganisationId;
            return context.Instances<User>().Where(u => u.OrganisationId == myOrgId && u.Role == Role.Student);
        }
    }
}