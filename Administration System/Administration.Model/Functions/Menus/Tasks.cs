namespace Model.Functions.Menus
{
    public static class Tasks
    {
        public static IQueryable<Task> AllTasks(IContext context) =>
            context.Instances<Task>().OrderBy(t => t.Title).ThenBy(t => t.Language);

        public static IQueryable<Task> AllPublicTasks(IContext context) =>
            AllTasks(context).Where(t => t.Status == TaskStatus.Public);

        public static IQueryable<Task> AllAssignableTasks(IContext context) =>
            AllTasks(context).Where(t => IsAssignable(t));

        public static IQueryable<Task> FindTasks(
            [Optionally] string title,
            [Optionally] ProgrammingLanguage? language,
            IContext context) =>
                AllTasks(context).Where(t =>
                    title == null || t.Title.ToUpper().Contains(title.ToUpper()) &&
                    (language == null || t.Language == language));

        public static IQueryable<Task> FindAssignableTasks(
            [Optionally] string title,
            [Optionally] ProgrammingLanguage? language,
            IContext context) =>
                FindTasks(title, language, context).Where(t => IsAssignable(t));

        private static bool IsAssignable(this Task t) => t.Status == TaskStatus.Public || t.Status == TaskStatus.MustBeAssigned;

        public static (Task, IContext) CreateNewTask(string title, IContext context)
        {
            var t = new Task() { Title = title, AuthorId = Users.Me(context).Id };
            return (t, context.WithNew(t));
        }

        [MemberOrder(20)]
        public static IQueryable<Task> TasksAuthoredByMe(IContext context)
        {
            var id = Users.Me(context).Id;
            return AllTasks(context).Where(t => t.AuthorId == id);
        }
    }
}
