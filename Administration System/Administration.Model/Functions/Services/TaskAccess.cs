namespace Model.Functions.Services;

public static class TaskAccess
{
    public static (CodeUserView, IContext) GetCodeVersion(int taskId, int codeVersion, IContext context)
    {
        var task = Tasks.GetTask(taskId, context);
        if (codeVersion > 0)
        {
            return ReturnCUVWithCodeVersion(taskId, codeVersion, context, task);
        }
        else
        {
            return CreateEmptyCUVForVersion0(taskId, codeVersion, context, task);
        }
    }

    private static (CodeUserView, IContext) ReturnCUVWithCodeVersion(int taskId, int codeVersion, IContext context, Task task)
    {
        var asgn = Assignments.GetAssignmentForCurrentUser(taskId, context);

        var successfulCodeCommits = Activities.ActivitiesOfCurrentUser(task.Id, context)
            .Where(a => a.ActivityType == ActivityType.SubmitCodeSuccess)
            .OrderByDescending(a => a.TimeStamp)
            .Skip(codeVersion - 1); //Because codeVersion 1 means 'most recently submitted code'

        var cuv = new CodeUserView(
            taskId,
            successfulCodeCommits.First().CodeSubmitted,
            codeVersion,
            successfulCodeCommits.Count() > 1
            );

        return (cuv, context);
    }

    private static (CodeUserView, IContext) CreateEmptyCUVForVersion0(int taskId, int codeVersion, IContext context, Task task)
    {
        if (MostRecentActivityOfType(ActivityType.SubmitCodeSuccess, task, context) is not null)
        {
            return (new CodeUserView(taskId, "", codeVersion, true), context);
        }
        else
        {
            return HandleCodeCarriedForwardIfAny(taskId, codeVersion, context, task);
        }
    }

    private static (CodeUserView, IContext) HandleCodeCarriedForwardIfAny(int taskId, int codeVersion, IContext context, Task task)
    {
        var codeCarriedForward = CodeCarriedForwardFromPriorTask(task, context);
        if (codeCarriedForward is null)
        {
            return (new CodeUserView(taskId, "", codeVersion, false), context);
        }
        else
        {
            var context2 = Activities.SubmitCodeSuccess(taskId, codeCarriedForward, context);
            return (new CodeUserView(taskId, "", codeVersion, true), context2);
        }
    }

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
                task.Hints.Any() ? $"{task.Hints.Count} hints available" : "No hints available for this task",
                null,
                0,
                NextHintNo(task, 0),
                0, //There is no longer any cost in marks to using hints
                NextHintIsAlreadyUsed(task, 0, context)
                );
            return (huv, context);
        }
        else
        {
            var context2 = IsCompleted(task, context) ? context : UseHintNo(task, hintNumber, context);
            var hint = task.GetHintNo(hintNumber);
            var huv = new HintUserView(
                taskId,
                hintNumber,
                hint.ToString(),
                hint.ContentsAsString(),
                hintNumber - 1,
                NextHintNo(task, hintNumber),
                0, //See above
                NextHintIsAlreadyUsed(task, hintNumber, context)
            );
            return (huv, context2);
        }
    }

    public static TaskUserView GetTask(int taskId, IContext context)
    {
        var task = Tasks.GetTask(taskId, context);
        var asgn = Assignments.GetAssignmentForCurrentUser(taskId, context);
        if (task == null || asgn == null)
        {
            return new TaskUserView(
                null,
                $"TASK {taskId} DOES NOT EXIST, OR IS NOT ASSIGNED TO YOU",
                false,
                false,
                0,
                false,
                false,
                null);
        }
        else
        {
            return new TaskUserView(
            task,
            Title(task, context),
            !task.HasTests() || IsCompleted(task, context),
            task.HasTests(),
            asgn.Id,
            IsStarted(task.NextTaskId, context),
            CanPaste(context),
            ClientRunTestCodeIfAny(task)
            );
        }
    }

    private static string ClientRunTestCodeIfAny(Task task) => task.TestsRunOnClient ? task.TestsFile.ContentsAsString() : null;

    internal static bool CanPaste(IContext context) => Users.UserRole(context) >= Role.Teacher;

    internal static string Title(Task task, IContext context) =>
        task.ToString() + (IsCompleted(task, context) ? $" COMPLETED" : "");

    internal static bool IsCompleted(Task task, IContext context) =>
       task.HasTests && Activities.ActivitiesOfCurrentUser(task.Id, context).FirstOrDefault()?.ActivityType == ActivityType.RunTestsSuccess;

    internal static bool IsStarted(int? taskId, IContext context) =>
        taskId is null ? false : Activities.ActivitiesOfCurrentUser(taskId.Value, context).Any();

    internal static string CodeCarriedForwardFromPriorTask(Task task, IContext context) =>
        task.PreviousTaskId is null ?
                null
                : task.PreviousTask.CodeCarriedForwardToNextTask() ?
                    MostRecentActivityOfType(ActivityType.RunTestsSuccess, task.PreviousTask, context)?.CodeSubmitted
                    : null;

    private static Activity MostRecentActivityOfType(ActivityType type, Task task, IContext context)
    {
        var asgn = Assignments.GetAssignmentForCurrentUser(task.Id, context);
        var activities = asgn.ListActivity(task.Id, context).Where(a => a.ActivityType == type);
        return activities.FirstOrDefault();
    }

    internal static int HighestHintNoUsed(Task task, IContext context) =>
        Activities.ActivitiesOfCurrentUser(task.Id, context).Select(a => a.HintUsed).ToList().DefaultIfEmpty(0).Max();

    internal static IContext UseHintNo(Task task, int hintNo, IContext context)
    {
        var asgn = Assignments.GetAssignmentForCurrentUser(task.Id, context);
        if (hintNo <= HighestHintNoUsed(task, context))
        {
            return context;
        }
        else
        {
            return Activities.RecordActivity(task.Id, ActivityType.HintUsed, null, null, hintNo, context);
        }
    }

    internal static int NextHintNo(Task task, int currentHintNumber) =>
        currentHintNumber < task.Hints.Count ? currentHintNumber + 1 : 0;

    internal static bool NextHintIsAlreadyUsed(Task task, int currentHintNo, IContext context)
    {
        var next = NextHintNo(task, currentHintNo);
        return next > 0 && next <= HighestHintNoUsed(task, context);
    }
}