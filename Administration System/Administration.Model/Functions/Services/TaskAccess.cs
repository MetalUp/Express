namespace Model.Functions.Services;

public static class TaskAccess {

    public static TaskUserView GetTask(int taskId, int withHintNo, IContext context)
    {
        var task = Tasks.GetTask(taskId, context);
        var asgn = Assignments.GetAssignmentForCurrentUser(taskId, context);
        if (asgn == null) return null;
        throw new NotFiniteNumberException();
        //return new TaskUserView();

    }

    public static bool Completed(Assignment asgn, Task task) => false;

    public static int HighestHintNoUsed(Assignment asgn, Task task) => 0;




}