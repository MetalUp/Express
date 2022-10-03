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

        [MemberOrder(10)]
        [Named("Return to Task view")]
        public Task Task { get; init; }

        [MemberOrder(10)]
        public virtual User Author => Task.Author;

        [MemberOrder(20)]
        public TaskStatus Status => Task.Status;

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
        [HideInClient]
        public bool PasteExpression => Task.PasteExpression;

        [MemberOrder(101)]
        [HideInClient]
        public bool PasteFunctions => Task.PasteFunctions;

        [MemberOrder(110)]
        public virtual Task PreviousTask => Task.PreviousTask;

        [MemberOrder(120)]
        public virtual Task NextTask => Task.NextTask;

        [MemberOrder(130)]
        public bool NextTaskClearsFunctions => Task.NextTaskClearsFunctions;

        [MemberOrder(140)]
        [MultiLine(10)]
        public string TeacherNotes => Task.TeacherNotes;

        [RenderEagerly]
        public virtual ICollection<Hint> Hints => Task.Hints;

        public override string ToString() => $"AUTHOR VIEW - {Title} ({Language})";
    }
}
