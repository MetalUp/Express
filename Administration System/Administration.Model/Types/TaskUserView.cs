namespace Model.Types;

[ViewModel(typeof(TaskUserView_Functions))]
public class TaskUserView
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TaskUserView() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public TaskUserView(Task? task, string title, bool completed, bool hasTests, int assignmentId, bool nextTaskIsStarted, bool canPaste, string? clientRunTestCode)
    {
        Task = task;
        Project = task?.Project;
        Title = title;
        Completed = completed;
        HasTests = hasTests;
        AssignmentId = assignmentId;
        NextTaskIsStarted = nextTaskIsStarted;
        PasteExpression = canPaste;
        PasteCode = canPaste;
        TestsRunLocally = clientRunTestCode != null;
        ClientRunTestCode = clientRunTestCode;
    }

    internal Task? Task { get; init; }

    internal Project? Project { get; init; }

    public string Title { get; init; }

    public string? Language => Task?.Language;

    public string? Description => Task?.DescriptionFile?.ContentsAsString();

    public string? RegExRules => Task?.RegExRules;

    public bool PasteExpression { get; init; }

    public bool PasteCode { get; init; }

    public bool Completed { get; init; }

    public int? PreviousTaskId => Task?.PreviousTaskId;

    public int? NextTaskId => Task?.NextTaskId;

    public bool NextTaskIsStarted { get; init; }

    public bool HasTests { get; init; }

    public bool TestsRunLocally { get; init; }

    public string? ClientRunTestCode { get; init; }

    public int AssignmentId { get; init; }
}