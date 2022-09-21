namespace Model.Functions.Menus
{
    public static class Activities
    {
        public static IQueryable<Activity> AllActivities(IContext context) => 
            context.Instances<Activity>().OrderByDescending(a => a.TimeStamp);
    }
}
