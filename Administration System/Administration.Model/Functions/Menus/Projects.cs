namespace Model.Functions.Menus
{
    public static class Projects
    {
        public static IQueryable<Project> AllProjects(
            [Optionally] ProgrammingLanguage? language,
            [Optionally] ProjectStatus? status,
            IContext context) =>
            context.Instances<Project>()
                .Where(t => (language == null || t.Language == language) &&
                (status == null || t.Status == status))
                .OrderBy(t => t.Title).ThenBy(t => t.Language);


        public static IQueryable<Project> AllAssignableProjects(
            [Optionally] ProgrammingLanguage? language, 
            IContext context) =>
            AllProjects(language, ProjectStatus.Assignable, context);

        public static IQueryable<Project> FindProjects(
            [Optionally] string title,
            [Optionally] ProgrammingLanguage? language,
            IContext context) =>
                AllAssignableProjects(language, context).Where(t =>
                    title == null || t.Title.ToUpper().Contains(title.ToUpper()) &&
                    (language == null || t.Language == language));


        public static (Project, IContext) CreateNewProject(string title, ProgrammingLanguage language, IContext context)
        {
            var p = new Project() { Title = title, Language = language, AuthorId = Users.Me(context).Id };
            return (p, context.WithNew(p));
        }

        [MemberOrder(20)]
        public static IQueryable<Project> ProjectsAuthoredByMe(
            [Optionally] ProgrammingLanguage? language,
            [Optionally] ProjectStatus? status,
            IContext context)
        {
            var id = Users.Me(context).Id;
            return AllProjects(language, status, context)
                .Where(t => t.AuthorId == id)
                .OrderBy(t => t.Status);
        }

        [MemberOrder(21)]
        public static IQueryable<Project> MyProjectsUnderDevelopment(IContext context) =>
            ProjectsAuthoredByMe(null, ProjectStatus.UnderDevelopment, context);
        
    }
}
