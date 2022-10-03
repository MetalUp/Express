namespace Model.Functions.Menus
{
    public static class Tasks
    {
        public static IQueryable<Task> AllTasks(
            [Optionally] ProgrammingLanguage? language,
            [Optionally] TaskStatus? status,
            IContext context) =>
            context.Instances<Task>()
                .Where(t => (language == null || t.Language == language) &&
                (status == null || t.Status == status))
                .OrderBy(t => t.Title).ThenBy(t => t.Language);

        public static IQueryable<Task> PublicTasks(
            [Optionally] ProgrammingLanguage? language, 
            IContext context) =>
            AllTasks(language, TaskStatus.Public, context);

        public static IQueryable<Task> AllAssignableTasks(
            [Optionally] ProgrammingLanguage? language, 
            IContext context) =>
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
        public static IQueryable<Task> TasksAuthoredByMe(
            [Optionally] ProgrammingLanguage? language,
            [Optionally] TaskStatus? status,
            IContext context)
        {
            var id = Users.Me(context).Id;
            return AllTasks(language, status, context)
                .Where(t => t.AuthorId == id)
                .OrderBy(t => t.Status);
        }

        [MemberOrder(21)]
        public static IQueryable<Task> MyTasksUnderDevelopment(IContext context) =>
            TasksAuthoredByMe(null, TaskStatus.UnderDevelopment, context);
        
    }
}
