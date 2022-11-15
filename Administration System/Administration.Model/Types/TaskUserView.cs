namespace Model.Types;

[ViewModel(typeof(TaskUserView_Functions))]
public class TaskUserView
{
    public TaskUserView(Task task, string title, string codeLastSubmitted, bool isCompleted)
    {
        Task = task;
        Project = task.Project;
        Title = title;
        CodeLastSubmitted = codeLastSubmitted;
        IsCompleted = isCompleted;
    }

    internal Task Task { get; init; }

    internal Project Project { get; init; }

    public string Title { get; init; }

    public string Language => Task.Language;

    public string Description => Task.DescriptionFile?.ContentsAsString();

    public string RegExRules => Project.RegExRules;

    public bool PasteExpression => Project.PasteExpression;

    public bool PasteCode => Task.Project.PasteCode;

    public bool IsCompleted { get; init; }

    public int? PreviousTaskId => Task.PreviousTaskId;

    public int? NextTaskId => Task.NextTaskId;

    public bool NextTaskClearsFunctions => Task.NextTaskClearsFunctions;


    public string CodeLastSubmitted { get; init; }

}