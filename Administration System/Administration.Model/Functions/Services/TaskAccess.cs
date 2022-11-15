namespace Model.Functions.Services;

public static class TaskAccess
{
    //If hintNumber is 0, will return EITHER an empty HintUserView, OR the highest level hint that the user previously accessed.
    public static (HintUserView, IContext) GetHint(int taskId, int hintNumber, IContext context)
    {
        if (hintNumber == 0)
        {
            return (null, context);
        }
        else {
            var asgn = Assignments.GetAssignmentForCurrentUser(taskId, context);
            if (asgn == null) return (null, context);
            var task = Tasks.GetTask(taskId, context);
            var context2 = UseHintNo(task, hintNumber, asgn, context);
            var hint = task.GetHintNo(hintNumber);
            var activities = asgn.ListActivity(context);
            var huv = new HintUserView(
                hint.ToString(),
                hint.ContentsAsString(),
                hintNumber - 1,
                NextHintNo(task, hintNumber),
                CostOfNextHint(task, activities, hintNumber)
            );
            return (huv, context2);
        }
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
        task.Title + (IsCompleted(task, activities) ? $" COMPLETED Final mark " : "Marks Available") + $"{MarksAvailable(task, activities)}/{task.MaxMarks}";
    
    private static int MarksAvailable(Task task, IQueryable<Activity> activities) =>
        task.MaxMarks - TotalMarksDeducted(task, activities);

    internal static bool IsCompleted(Task task, IQueryable<Activity> activities) =>
      activities.LastOrDefault()?.ActivityType == ActivityType.RunTestsSuccess;

    internal static bool NextTaskEnabled(Task task, IQueryable<Activity> activities) =>
        IsCompleted(task, activities);

    internal static string CodeLastSubmitted(Task task, IQueryable<Activity> activities) =>
        activities.Where(a => a.CodeSubmitted != null).LastOrDefault()?.CodeSubmitted;

    internal static int HighestHintNoUsed(Task task, IQueryable<Activity> activities) =>
        activities.Select(a => a.HintUsed).DefaultIfEmpty(0).Max();

    internal static int TotalMarksDeducted(Task task, IQueryable<Activity> activities)
    {
        var highest = HighestHintNoUsed(task, activities);
        return task.Hints.Where(h => h.Number <= highest).Select(h => h.Number).DefaultIfEmpty(0).Sum();
    }

    internal static IContext UseHintNo(Task task, int hintNo, Assignment asgn, IContext context)
    {
        var activities = asgn.ListActivity(context);
        if ( hintNo <= HighestHintNoUsed(task, activities))
        {
            return context;
        } else
        {
            var act = new Activity(asgn.Id, task.Id, ActivityType.HintUsed, hintNo, null, null, context);
            return context.WithNew(act);
        }          
    }

    internal static int NextHintNo(Task task, int currentHintNumber) =>
        currentHintNumber < task.Hints.Count ? currentHintNumber + 1 : 0;

    internal static int CostOfNextHint(Task task, IQueryable<Activity> activities, int currentHintNo) =>
        currentHintNo == HighestHintNoUsed(task, activities) ? task.GetHintNo(NextHintNo(task, currentHintNo)).CostInMarks : 0;
}