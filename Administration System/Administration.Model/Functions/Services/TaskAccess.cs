﻿namespace Model.Functions.Services;

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
                null,
                NextHintNo(task, 0),
                task.Hints.Any() ? task.GetHintNo(1).CostInMarks : 0);
            return (huv, context);
        }
        else
        {
            var context2 = UseHintNo(task, hintNumber, asgn, context);
            var hint = task.GetHintNo(hintNumber);
            var activities = asgn.ListActivity(context);
            var huv = new HintUserView(
                taskId, 
                hintNumber,
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
           CodeForTask(task, activities, context),
           IsCompleted(task, activities) || !task.HasTests(),
           task.Tests is not null
           );
    }

    internal static string Title(Task task, IQueryable<Activity> activities) =>
        task.Title + (IsCompleted(task, activities) ? $" COMPLETED Final mark " : "Marks Available") + $"{MarksAvailable(task, activities)}/{task.MaxMarks}";

    private static int MarksAvailable(Task task, IQueryable<Activity> activities) =>
        task.MaxMarks - TotalMarksDeducted(task, activities);

    internal static bool IsCompleted(Task task, IQueryable<Activity> activities) =>
       task.HasTests && activities.LastOrDefault()?.ActivityType == ActivityType.RunTestsSuccess;

    internal static string CodeForTask(Task task, IQueryable<Activity> activities, IContext context) =>
        IsCompleted(task, activities) ?
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



    //Rename to StartingCodeForTask
    // If task is completed, it is the code last submitted. If Task not completed, and no code submitted, and Task says carry forward the code,
    //then code last submitted from previous task

    internal static int HighestHintNoUsed(Task task, IQueryable<Activity> activities) =>
        activities.Select(a => a.HintUsed).ToList().DefaultIfEmpty(0).Max();

    internal static int TotalMarksDeducted(Task task, IQueryable<Activity> activities)
    {
        var highest = HighestHintNoUsed(task, activities);
        return task.Hints.Where(h => h.Number <= highest).Select(h => h.Number).ToList().DefaultIfEmpty(0).Sum();
    }

    internal static IContext UseHintNo(Task task, int hintNo, Assignment asgn, IContext context)
    {
        var activities = asgn.ListActivity(context);
        if (hintNo <= HighestHintNoUsed(task, activities))
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

    internal static int CostOfNextHint(Task task, IQueryable<Activity> activities, int currentHintNo) =>
        currentHintNo == HighestHintNoUsed(task, activities) ? task.GetHintNo(NextHintNo(task, currentHintNo)).CostInMarks : 0;
}