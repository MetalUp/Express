namespace Model.Functions.Services;

public static class TaskAccess
{

    public static TaskUserView GetTask(int taskId, int currentHintNo, IContext context)
    {
        var task = Tasks.GetTask(taskId, context);
        var asgn = Assignments.GetAssignmentForCurrentUser(taskId, context);
        var activities = asgn.ListActivity(context);
        if (asgn == null) return null;
        var hint = GetHintNo(task, activities, currentHintNo);
        return new TaskUserView(
            task,
            Title(task, activities),
            NextTaskEnabled(task, activities),
            CodeLastSubmitted(task, activities),
            currentHintNo,
            CurrentHintTitle(task, activities),
            hint.ContentsAsString(),
            PreviousHintNo(task, currentHintNo),
            NextHintNo(task, currentHintNo),
            CostOfNextHint(task, currentHintNo)
            );
    }

    //public TaskUserView(Task task, bool completed, string codeLastSubmitted,
    // int currentHintNo, string currentHintTitle, string currentHintContent,
    // int? previousHintNo, int? nextHintNo, int costOfNextHint)
    //{

    internal static string Title(Task task, IQueryable<Activity> activities) => "";

    internal static bool NextTaskEnabled(Task task, IQueryable<Activity> activities) => false;

    internal static int HighestHintNoUsed(Task task, IQueryable<Activity> activities) => 0;

    internal static int TotalMarksDeducted(Task task, IQueryable<Activity> activities) => 0;

    internal static Hint GetHintNo(Task task, IQueryable<Activity> activities, int hintNo) => null;
    //Needs to check if user has previously accessed this hint no. If not record the hint access

    internal static string CurrentHintTitle(Task task, IQueryable<Activity> activities) => "";

    internal static string CodeLastSubmitted(Task task, IQueryable<Activity> activities) => null;

    internal static int PreviousHintNo(Task task, int currentHintNumber) => 0;

    internal static int NextHintNo(Task task, int currentHintNumber) => 0;

    internal static int CostOfNextHint(Task task, int currentHintNumber) => 0;










}