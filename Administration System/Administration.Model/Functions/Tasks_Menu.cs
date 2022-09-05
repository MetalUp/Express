
namespace Model.Functions
{
    [Named("Tasks")]
    public static class Task_Menu
    {
        public static (Task, IContext) CreateNewTask(string title, ProgrammingLanguage language, IContext context)
        {
            var t = new Task { Title = title, Language = language};
            return (t, context.WithNew(t));
        }

        public static string ValidateCreateNewTask(string title, ProgrammingLanguage language, IContext context) =>
            context.Instances<Task>().Any(t => t.Title.ToUpper() == title.ToUpper() && t.Language == language) ?
                "There already exists a Task with this Name and Language" : "";

        public static IQueryable<Task> AllTasks(IContext context) =>context.Instances<Task>();
    }
}
