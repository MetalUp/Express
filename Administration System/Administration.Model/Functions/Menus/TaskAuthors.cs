
namespace Model.Functions.Menus
{
    public static class TaskAuthors
    {
        [MemberOrder(10)]
        public static IQueryable<Task> AllTasks(IContext context) => TaskRepository.AllTasks(context);

        [MemberOrder(20)]
        public static IQueryable<Task> TasksAuthoredByMe(IContext context) {
            var id = UserRepository.Me(context).Id;
            return TaskRepository.AllTasks(context).Where(t => t.AuthorId == id);
        }


        [MemberOrder(30)]
        public static IQueryable<Task> FindTasks(
            [Optionally] string title,
            [Optionally] ProgrammingLanguage? language,
            IContext context) =>
            TaskRepository.FindTasks(title, language, context);

        [CreateNew][MemberOrder(40)]
        public static (Task, IContext) CreateNewTask(string title, IContext context) => TaskRepository.CreateNewTask(title, context);
    }
}
