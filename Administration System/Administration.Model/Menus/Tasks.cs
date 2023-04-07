
namespace Model.Menus
{
    public static class Tasks
    {
        //[UrlLink]
        [Named("Go to Task No.")]
        [MemberOrder(10)]
        public static string GoToTaskNo([Named("Task No.")] int taskId) =>  $"/task/{taskId}";

        //[UrlLink]
        [MemberOrder(20)]
        public static string GoToLastActiveTask(IContext context) => GoToTaskNo(MyLastActiveTaskId(context));

        public static bool HideGoToLastActiveTask(IContext context) => MyLastActiveTaskId(context) == 0;

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
