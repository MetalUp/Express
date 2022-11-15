namespace Model.Functions.Services;

public static class TaskAccess
{
    //If hintNumber is 0, will return EITHER an empty HintUserView, OR the highest level hint that the user previously accessed.
    public static (HintUserView, IContext) GetHint(int taskId, int hintNumber)
    {
        throw new NotImplementedException();
    }

    public static TaskUserView GetTask(int taskId, IContext context)
    {
        var asgn = Assignments.GetAssignmentForCurrentUser(taskId, context);
        if (asgn == null) return null;
        var activities = asgn.ListActivity(context);
        var task = Tasks.GetTask(taskId, context);
        return new TaskUserView(
           task,
           Title(task, activities),
           NextTaskEnabled(task, activities),
           CodeLastSubmitted(task, activities)
           );
    }

    internal static string Title(Task task, IQueryable<Activity> activities) =>
        task.Title + (IsCompleted(task, activities) ? " COMPLETED" : "");

    internal static bool IsCompleted(Task task, IQueryable<Activity> activities) =>
        activities.Last().ActivityType == ActivityType.RunTestsSuccess;

    internal static bool NextTaskEnabled(Task task, IQueryable<Activity> activities) =>
        IsCompleted(task, activities);

    internal static int HighestHintNoUsed(Task task, IQueryable<Activity> activities) =>
        activities.Max(a => a.HintUsed);

    internal static int TotalMarksDeducted(Task task, IQueryable<Activity> activities)
    {
        var highest = HighestHintNoUsed(task, activities);
        return task.Hints.Where(h => h.Number <= highest).Sum(h => h.CostInMarks);
    }

    internal static IContext UseHintNo(int taskId, int hintNo, IContext context)
    {
        var asgn = Assignments.GetAssignmentForCurrentUser(taskId, context);
        if (asgn == null) return context;
        var activities = asgn.ListActivity(context);
        var task = Tasks.GetTask(taskId, context);
        if ( hintNo <= HighestHintNoUsed(task, activities))
        {
            return context;
        } else
        {
            var act = new Activity(asgn.Id, task.Id, ActivityType.HintUsed, hintNo, null, null, context);
            return context.WithNew(act);
        }
          
    }

    internal static string CodeLastSubmitted(Task task, IQueryable<Activity> activities) =>
        activities.Last(a => a.CodeSubmitted != null).CodeSubmitted;

    internal static int PreviousHintNo(Task task, int currentHintNumber) =>
        currentHintNumber > 1 ? currentHintNumber - 1 : 0;

    internal static int NextHintNo(Task task, int currentHintNumber) =>
        currentHintNumber < task.Hints.Count ? currentHintNumber + 1 : 0;

    internal static int CostOfNextHint(Task task, IQueryable<Activity> activities, int currentHintNumber)
    {
        var next = NextHintNo(task, currentHintNumber);
        return next > HighestHintNoUsed(task, activities) ? task.GetHintNo(next).CostInMarks : 0;

    }
}