
namespace Model.Functions.Menus
{
    public static class Tasks
    {
        internal static Task GetTask(int taskId, IContext context) => context.Instances<Task>().SingleOrDefault(t => t.Id == taskId);

        //Defined as the TaskId on the user's most recent activity on a current assignment
        //If no such task, returns 0
        internal static int MyLastActiveTaskId(IContext context)
        {
            var last = Activities.ActivitiesOfCurrentUser(context).FirstOrDefault();
            return last is null ? 0 : last.TaskId;
        }
    }
}
