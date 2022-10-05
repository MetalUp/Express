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
        public FileAttachment Description => (Task.DescContent == null) ? null :
                 new FileAttachment(Task.DescContent, Task.DescName, Task.DescMime);

        [MemberOrder(80)]
        [Named("Hidden Functions")]
        public FileAttachment ReadyMadeFunctions => (Task.RMFContent == null) ? null :
                 new FileAttachment(Task.RMFContent, Task.RMFName, Task.RMFMime);

        [MemberOrder(90)]
        public FileAttachment Tests => (Task.TestsContent == null) ? null :
                 new FileAttachment(Task.TestsContent, Task.TestsName, Task.TestsMime);

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

        [RenderEagerly]
        public virtual ICollection<Hint> Hints => Task.Hints;

        public override string ToString() => $"AUTHOR VIEW - {Title} ({Language})";
    }
}
