namespace Model.Types;

[ViewModel(typeof(TaskUserView_Functions))]
public class TaskUserView
{
    public TaskUserView(Task task, bool completed, string codeLastSubmitted,
        int currentHintNo, string currentHintTitle, string currentHintContent, 
        int? previousHintNo, int? nextHintNo, int costOfNextHint)
    {
        Task = task;
        Project = task.Project;
        CodeLastSubmitted = codeLastSubmitted;
        PreviousHintNo = previousHintNo;
        CurrentHintNo = currentHintNo;
        CurrentHintTitle = currentHintTitle;
        CurrentHintContent = currentHintContent;
        NextHintNo = nextHintNo;
    }

    internal Task Task { get; init; }

    internal Project Project { get; init; }

    public string Title => ""; //TODO: Task title, whether completed and marks (Max, then remaining, then final score)

    public string Language => Task.Language;

    public string Description => Task.DescriptionFile?.ContentsAsString();

    public string RegExRules => Project.RegExRules;

    public bool PasteExpression => Project.PasteExpression;

    public bool PasteCode => Task.Project.PasteCode;

    public int? PreviousTaskId => Task.PreviousTaskId;

    public int? NextTaskId => Task.NextTaskId;

    public bool NextTaskClearsFunctions => Task.NextTaskClearsFunctions;

    public bool Completed { get; init; } //Derrived from Activities

    public string CodeLastSubmitted { get; init; }  //Derrived from Activities

    public int? PreviousHintNo { get; init; }

    public int CurrentHintNo { get; init; } //The hint current being displayed (not necessarily the highest hint accessed so far)

    public string CurrentHintTitle { get; init; } //Includes hint number & 'out of' e.g. Hint 3/5 & Next hint costs 1

    public string CurrentHintContent { get; init; }

    public int? NextHintNo { get; init; }



}