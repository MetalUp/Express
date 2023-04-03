
namespace Model.Functions.Menus
{
    public static class Tasks
    {
        internal static Task GetTask(int taskId, IContext context) => context.Instances<Task>().SingleOrDefault(t => t.Id == taskId);

        //Defined as the TaskId on the user's most recent activity on a current assignment
        //If no such task, returns 0
        internal static int MyLastActiveTaskId(IContext context)
        {
            return (from a in Assignments.MyCurrentAssignments(context)
                    from act in Activities.AllActivities(context)  //these are ordered most recent first
                    where act.AssignmentId == a.Id
                    select act.TaskId).FirstOrDefault();
        }
    }
}
