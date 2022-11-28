namespace Model.Types;

[ViewModel(typeof(TaskUserView_Functions))]
public class TaskUserView
{
    public TaskUserView(Task task, string title, string codeLastSubmitted, bool nextTaskEnabled, bool hasTests, int assignmentId, bool completed)
    {
        Task = task;
        Project = task.Project;
        Title = title;
        Code = codeLastSubmitted;
        NextTaskIsStarted = nextTaskEnabled;
        HasTests = hasTests;
        AssignmentId = assignmentId;
        Completed = completed;
    }

    internal Task Task { get; init; }

    internal Project Project { get; init; }

    public string Title { get; init; }

    public string Language => Task.Language;

    public string Description => Task.DescriptionFile?.ContentsAsString();

    public string RegExRules => Project.RegExRules;

    public bool PasteExpression => Project.PasteExpression;

    public bool PasteCode => Task.Project.PasteCode;

    public bool Completed { get; init; }

    public int? PreviousTaskId => Task.PreviousTaskId;

    public int? NextTaskId => Task.NextTaskId;

    public bool NextTaskIsStarted { get; init; }

    public string Code { get; init; } //Might be carried forward from previous, or 

    public bool HasTests { get; init; }

    public int AssignmentId { get; init; }
}