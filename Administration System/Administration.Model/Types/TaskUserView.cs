namespace Model.Types;

[ViewModel(typeof(TaskUserView_Functions))]
public class TaskUserView
{
    public TaskUserView(Task task, string title, bool nextTaskEnabled, string codeLastSubmitted)
    {
        Task = task;
        Project = task.Project;
        Title = title;
        NextTaskEnabled = nextTaskEnabled;
        CodeLastSubmitted = codeLastSubmitted;
    }

    internal Task Task { get; init; }

    internal Project Project { get; init; }

    public string Title { get; init; }

    public string Language => Task.Language;

    public string Description => Task.DescriptionFile?.ContentsAsString();

    public string RegExRules => Project.RegExRules;

    public bool PasteExpression => Project.PasteExpression;

    public bool PasteCode => Task.Project.PasteCode;

    public int? PreviousTaskId => Task.PreviousTaskId;

    public int? NextTaskId => Task.NextTaskId;

    public bool NextTaskClearsFunctions => Task.NextTaskClearsFunctions;

    public bool NextTaskEnabled { get; init; }

    public string CodeLastSubmitted { get; init; }  //Derrived from Activities
}