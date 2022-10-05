using NakedFramework.Value;

namespace Model.Types
{
    [ViewModel(typeof(Task_Author_Functions))]
    public class TaskAuthorView
    {
        public TaskAuthorView()
        {

        }
        public TaskAuthorView(Task task)
        {
            Task = task;
        }

        [Hidden]
        public Task Task { get; init; }

        [MemberOrder(30)]
        public string Title => Task.Title;

        [MemberOrder(40)]
        public ProgrammingLanguage Language => Task.Language;

        //Marks awarded for completing the task with no hints taken
        [MemberOrder(60)]
        public int MaxMarks => Task.MaxMarks;

        [MemberOrder(70)]
        public FileAttachment Description => (Task.DescContent == null) ? null : Task.Description;

        [MemberOrder(80)]
        [Named("Hidden Functions")]
        public FileAttachment ReadyMadeFunctions => (Task.RMFContent == null) ? null : Task.ReadyMadeFunctions;

        [MemberOrder(90)]
        public FileAttachment Tests => (Task.TestsContent == null) ? null : Task.Tests;

        [MemberOrder(100)]
        public bool PasteExpression => Task.PasteExpression;

        [MemberOrder(101)]
        public bool PasteFunctions => Task.PasteFunctions;

        [MemberOrder(110)]
        public virtual Task PreviousTask => Task.PreviousTask;

        [MemberOrder(120)]
        public virtual Task NextTask => Task.NextTask;

        [MemberOrder(130)]
        public bool NextTaskClearsFunctions => Task.NextTaskClearsFunctions;

        [MemberOrder(140)]
        public Project Project => Task.Project;

        [RenderEagerly]
        public virtual ICollection<Hint> Hints => Task.Hints;


        public override string ToString() => $"AUTHOR VIEW - {Title} ({Language})";
    }
}
