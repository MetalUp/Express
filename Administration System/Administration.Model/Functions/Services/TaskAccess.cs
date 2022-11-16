namespace Model.Functions.Services;

public static class TaskAccess
{
    public static (HintUserView, IContext) GetHint(int taskId, int hintNumber, IContext context)
    {
        var asgn = Assignments.GetAssignmentForCurrentUser(taskId, context);
        if (asgn == null) return (null, context);
        var task = Tasks.GetTask(taskId, context);
        if (hintNumber == 0)
        {
            var huv = new HintUserView(
                taskId,
                hintNumber,
                task.Hints.Any() ? $"{task.Hints.Count} hints available" : "No hints for this task",
                null,
                0,
                NextHintNo(task, 0),
                task.Hints.Any() ? task.GetHintNo(1).CostInMarks : 0);
            return (huv, context);
        }
        else
        {
            var context2 = UseHintNo(task, hintNumber, context);
            var hint = task.GetHintNo(hintNumber);
            var huv = new HintUserView(
                taskId,
                hintNumber,
                hint.ToString(),
                hint.ContentsAsString(),
                hintNumber - 1,
                NextHintNo(task, hintNumber),
                CostOfNextHint(task, hintNumber, context)
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
           Title(task, context),
           CodeForTask(task, context),
           !task.HasTests() ||IsCompleted(task, context),
           task.Tests is not null,
           asgn.Id
           );
    }

    internal static string Title(Task task, IContext context) =>
        task.Title +
            (IsCompleted(task, context) ? $" COMPLETED Final mark " : " Marks Available ") +
                $"{MarksAvailable(task, context)}/{task.MaxMarks}";

    private static int MarksAvailable(Task task, IContext context) =>
        task.MaxMarks - TotalMarksDeducted(task, context);

    internal static bool IsCompleted(Task task, IContext context) =>
       task.HasTests && Activities.ActivitiesOfCurrentUser(task.Id, context).LastOrDefault()?.ActivityType == ActivityType.RunTestsSuccess;

    internal static string CodeForTask(Task task, IContext context) =>
        IsCompleted(task, context) ?
            CodeLastSubmitted(task, context)
            : task.PreviousTaskId is null ?
                null
                : task.PreviousTask.CodeCarriedForwardToNextTask() ?
                    CodeLastSubmitted(task.PreviousTask, context)
                    : null;

    private static string CodeLastSubmitted(Task task, IContext context)
    {
        var asgn = Assignments.GetAssignmentForCurrentUser(task.Id, context);
        var activities = asgn.ListActivity(context);
        return activities.Where(a => a.CodeSubmitted != null).LastOrDefault()?.CodeSubmitted;
    }

    internal static int HighestHintNoUsed(Task task, IContext context) =>
        Activities.ActivitiesOfCurrentUser(task.Id, context).Select(a => a.HintUsed).ToList().DefaultIfEmpty(0).Max();

    internal static int TotalMarksDeducted(Task task, IContext context)
    {
        var highest = HighestHintNoUsed(task, context);
        return task.Hints.Where(h => h.Number <= highest).Select(h => h.CostInMarks).ToList().DefaultIfEmpty(0).Sum();
    }

    internal static IContext UseHintNo(Task task, int hintNo, IContext context)
    {
        var asgn = Assignments.GetAssignmentForCurrentUser(task.Id, context);
        if (hintNo <= HighestHintNoUsed(task, context))
        {
            return context;
        }
        else
        {
            var act = new Activity(asgn.Id, task.Id, ActivityType.HintUsed, hintNo, null, null, context);
            return context.WithNew(act);
        }
    }

    internal static int NextHintNo(Task task, int currentHintNumber) =>
        currentHintNumber < task.Hints.Count ? currentHintNumber + 1 : 0;

    internal static int CostOfNextHint(Task task, int currentHintNo, IContext context) =>
        NextHintNo(task, currentHintNo) == 0 ?
            0
            : currentHintNo == HighestHintNoUsed(task, context) ?
                task.GetHintNo(NextHintNo(task, currentHintNo)).CostInMarks
                : 0;
}