
namespace Model.Functions.Menus
{
    public static class Tasks
    {
        internal static Task GetTask(int taskId, IContext context) => context.Instances<Task>().SingleOrDefault(t => t.Id == taskId);
    }
}
