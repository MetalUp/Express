namespace Model.Functions.Menus
{
    public static class Groups
    {
        public static IQueryable<Group> AllOurGroups(IContext context)
        {
            var id = Organisations.MyOrganisation(context).Id;
            return context.Instances<Group>().Where(g => g.OrganisationId == id).OrderBy(g => g.Name);
        }
           
        [CreateNew]
        public static (Group, IContext) CreateNewGroup(string name, IContext context)
        {
            var org = Organisations.MyOrganisation(context);
            var g = new Group() { Name = name, OrganisationId = org.Id, Organisation = org };
            return (g, context.WithNew(g));
        }

        public static IQueryable<Group> AllGroups(IContext context) =>
            context.Instances<Group>();
    }
}
