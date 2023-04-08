namespace Model.Types;

[ViewModel(typeof(TaskProgress_Functions))]
public class TaskProgress
{
    public TaskProgress(int assignmentId, int taskId, string taskNoOrSummary, string status)
    {
        AssignmentId = assignmentId;
        TaskId = taskId;
        TaskNoOrSummary = taskNoOrSummary;
        Status = status;
    }

    internal int AssignmentId { get; init; }

    internal int TaskId { get; init; }

    public string TaskNoOrSummary { get; init; }

    public string Status { get; init; }

}