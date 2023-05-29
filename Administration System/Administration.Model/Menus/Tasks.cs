
namespace Model.Menus
{
    public static class Tasks
    {
        [UrlLink(true)]
        [Named("Go to Task No.")]
        [MemberOrder(10)]
        public static string GoToTaskNo([Named("Task No.")] int taskId) =>  $"/task/{taskId}";

        [UrlLink(true)]
        [MemberOrder(20)]
        public static string GoToLastActiveTask(IContext context) => GoToTaskNo(MyLastActiveTaskId(context));

        [TableView(false, nameof(Task.Project), nameof(Task.Number), nameof(Task.Title))]
        [RenderEagerly]
        public static IQueryable<Task> AllTasks(IContext context) => context.Instances<Task>().OrderBy(t => t.ProjectId).ThenBy(t => t.Number);

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
