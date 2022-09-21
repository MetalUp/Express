namespace Model.Functions.Menus
{
    public static class Tasks
    {
        internal static IQueryable<Task> AllTasks(
            ProgrammingLanguage? language, 
            TaskStatus? status,
            IContext context) =>
            context.Instances<Task>()
                .Where(t => (language == null || t.Language == language) &&
                (status == null || t.Status == status))
                .OrderBy(t => t.Title).ThenBy(t => t.Language);

        public static IQueryable<Task> PublicTasks( ProgrammingLanguage? language, IContext context) =>
            AllTasks(language, TaskStatus.Public, context);

        public static IQueryable<Task> AllAssignableTasks(ProgrammingLanguage? language, IContext context) =>
            AllTasks(language, TaskStatus.Assignable, context);

        public static IQueryable<Task> FindTasks(
            [Optionally] string title,
            [Optionally] ProgrammingLanguage? language,
            IContext context) =>
                AllAssignableTasks(language, context).Where(t =>
                    title == null || t.Title.ToUpper().Contains(title.ToUpper()) &&
                    (language == null || t.Language == language));


        public static (Task, IContext) CreateNewTask(string title, IContext context)
        {
            var t = new Task() { Title = title, AuthorId = Users.Me(context).Id };
            return (t, context.WithNew(t));
        }

        [MemberOrder(20)]
        public static IQueryable<Task> TasksAuthoredByMe(IContext context,  
            ProgrammingLanguage? language, 
            TaskStatus? status)
        {
            var id = Users.Me(context).Id;
            return context.Instances<Task>()
                .Where(t => t.AuthorId == id)
                .OrderBy(t => t.Status);
        }

        [MemberOrder(30)]
        public static IQueryable<Task> TasksUnderDevelopment(IContext context) =>
            context.Instances<Task>()
                .Where(t => t.Status == TaskStatus.UnderDevelopment)
                .OrderBy(t => t.Title).ThenBy(t => t.Language);
    }
}
