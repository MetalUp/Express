namespace Model.Functions
{
    public static class Task_Functions
    {
        internal static bool IsDefault(this Task task) => task.Project.Status == ProjectStatus.Public;

        public static TaskAuthorView AuthorView(this Task task) => new TaskAuthorView(task);
    }
}
