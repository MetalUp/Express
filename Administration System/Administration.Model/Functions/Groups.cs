namespace Model.Functions
{

    [Named("Groups")]
    public static class Group_MenuFunctions
    {
        public static (Group, IContext) CreateNewGroup(string name, IContext context)
        {
            var g = new Group { GroupName = name };
            return (g, context.WithNew(g));
        }

        public static IQueryable<Group> AllGroups(IContext context) => context.Instances<Group>();
    }

    public static class Group_Functions
    {
        //Assign Students (or Teachers) associated from the organisation, or from another group 
        //De-assign students (or teachers) from group
        //Edit group name / description
    }
}