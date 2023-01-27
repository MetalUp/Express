namespace Model.Functions.Menus
{
    public static class Projects
    {

        [MemberOrder(10)]
        public static IQueryable<Project> ProjectsAuthoredByMe(IContext context)
        {
            var id = Users.Me(context).Id;
            return AllProjects(context).Where(t => t.AuthorId == id);
        }

        [MemberOrder(20)]
        [TableView(false, "Status", "Title", "Language")]
        public static IQueryable<Project> AllProjects(IContext context) =>
            context.Instances<Project>().OrderBy(p => p.Status).ThenBy(p => p.Title).ThenBy(p => p.Language);

        [MemberOrder(30)]
        public static (Project, IContext) CreateNewProject(string title, Language language, IContext context)
        {
            var p = new Project() { Title = title, Language = language, AuthorId = Users.Me(context).Id };
            return (p, context.WithNew(p));
        }

        [MemberOrder(120)]
        public static IQueryable<Project> AllAssignableProjects(
            [Optionally] Language language,
            IContext context) =>
            AllProjects(context).Where(p => p.Status == ProjectStatus.Assignable && (language == null || p.Language == language));


        [MemberOrder(130)]
        public static IQueryable<Project> FindProjects(
             Language language,
            [Optionally] [DescribedAs("optional")]string title,
            IContext context) =>
                AllAssignableProjects(language, context).Where(t => title == null || t.Title.ToUpper().Contains(title.ToUpper()));
    }
}
