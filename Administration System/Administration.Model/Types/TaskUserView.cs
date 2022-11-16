namespace Model.Types;

[ViewModel(typeof(TaskUserView_Functions))]
public class TaskUserView
{
    public TaskUserView(Task task, string title, string codeLastSubmitted, bool nextTaskEnabled, bool hasTests)
    {
        Task = task;
        Project = task.Project;
        Title = title;
        Code = codeLastSubmitted;
        NextTaskEnabled = nextTaskEnabled;
        HasTests = hasTests;
    }

    internal Task Task { get; init; }

    internal Project Project { get; init; }

    public string Title { get; init; }

    public string Language => Task.Language;

    public string Description => Task.DescriptionFile?.ContentsAsString();

    public string RegExRules => Project.RegExRules;

    public bool PasteExpression => Project.PasteExpression;

    public bool PasteCode => Task.Project.PasteCode;

    public bool NextTaskEnabled { get; init; } //Rename to NextTask is navigable or somesuch

    public int? PreviousTaskId => Task.PreviousTaskId;

    public int? NextTaskId => Task.NextTaskId;

    public string Code { get; init; } //Might be carried forward from previous, or 

    public bool HasTests { get; init; }

}