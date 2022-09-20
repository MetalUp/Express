namespace Model.Functions
{
    public static class TaskRepository
    {
        public static IQueryable<Task> AllTasks(IContext context) =>
            context.Instances<Task>().OrderBy(t => t.Title).ThenBy(t => t.Language);

        public static IQueryable<Task> FindTasks(
            [Optionally] string title, 
            [Optionally] ProgrammingLanguage? language,
            IContext context) =>
                AllTasks(context).Where(t => 
                    (title == null || t.Title.ToUpper().Contains(title.ToUpper()) &&
                    (language == null || t.Language == language)));

        [CreateNew]
        public static (Task, IContext) CreateNewTask(string title, IContext context)
        {
            var t = new Task() { Title = title, AuthorId = UserRepository.Me(context).Id };
            return (t, context.WithNew(t));
        }
    }
}
