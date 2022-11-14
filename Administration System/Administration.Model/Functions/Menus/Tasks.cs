
namespace Model.Functions.Menus
{
    public static class Tasks
    {
        internal static Task GetTask(int taskId, IContext context) => context.Instances<Task>().Single(t => t.Id == taskId);


    }
}
