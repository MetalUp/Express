namespace Model.Functions.Services;

public static class TaskAccess {
    public static Task GetTask(int taskId, IContext context) => context.Instances<Task>().Single(t => t.Id == taskId);
}