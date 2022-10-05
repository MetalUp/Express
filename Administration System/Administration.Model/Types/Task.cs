using NakedFramework.Value;

namespace Model.Types
{
    public class Task
    {
        public Task() { }
        public Task(Task cloneFrom)
        {
            Id = cloneFrom.Id;
            Title = cloneFrom.Title;
            MaxMarks = cloneFrom.MaxMarks;
            DescContent = cloneFrom.DescContent;
            DescName = cloneFrom.DescName;
            DescMime = cloneFrom.DescMime;
            RMFContent = cloneFrom.RMFContent;
            RMFName = cloneFrom.RMFName;
            RMFMime = cloneFrom.RMFMime;
            PasteExpression = cloneFrom.PasteExpression;
            PasteFunctions = cloneFrom.PasteFunctions;
            TestsContent = cloneFrom.TestsContent;
            TestsName = cloneFrom.TestsName;
            TestsMime = cloneFrom.TestsMime;
            PreviousTaskId = cloneFrom.PreviousTaskId;
            PreviousTask = cloneFrom.PreviousTask;
            NextTaskId = cloneFrom.NextTaskId;
            NextTask = cloneFrom.NextTask;
            NextTaskClearsFunctions = cloneFrom.NextTaskClearsFunctions;
            Hints = cloneFrom.Hints;
        }

        [Hidden]
        public int Id { get; init; }

        [Hidden]
        public int? ProjectId { get; init; }
        [Hidden]
        public virtual Project Project { get; init; }

        [MemberOrder(10)]
        [UrlLink("Try out the Task")]
        public string Link => $"https://express.metalup.org/task/{Id}";

        [MemberOrder(30)]
        public string Title { get; init; }

        [MemberOrder(40)]
        public ProgrammingLanguage Language => Project.Language;

        //Marks awarded for completing the task with no hints taken
        [MemberOrder(60)]
        public int MaxMarks { get; init; }

        #region Description
        [HideInClient]
        public FileAttachment Description => (DescContent == null) ? null :
                 new FileAttachment(DescContent, DescName, DescMime);

        [Hidden]
        public byte[] DescContent { get; init; }

        [Hidden]
        public string DescName { get; init; }

        [Hidden]
        public string DescMime { get; init; }
        #endregion

        #region ReadyMadeFunctions
        [Named("Hidden Functions")]
        [HideInClient]
        public FileAttachment ReadyMadeFunctions => (RMFContent == null) ? null :
                 new FileAttachment(RMFContent, RMFName, RMFMime);

        [Hidden]
        public byte[] RMFContent { get; init; }

        [Hidden]
        public string RMFName { get; init; }

        [Hidden]
        public string RMFMime { get; init; }
        #endregion

        #region Tests
        [HideInClient]
        public FileAttachment Tests => (TestsContent == null) ? null :
                 new FileAttachment(TestsContent, TestsName, TestsMime);

        [Hidden]
        public byte[] TestsContent { get; init; }

        [Hidden]
        public string TestsName { get; init; }

        [Hidden]
        public string TestsMime { get; init; }
        #endregion

        [MemberOrder(100)]
        [HideInClient]
        public bool PasteExpression { get; init; }

        [MemberOrder(101)]
        [HideInClient]
        public bool PasteFunctions { get; init; }

        [Hidden]
        public int? PreviousTaskId { get; init; }

        [MemberOrder(110)]
        public virtual Task PreviousTask { get; init; }

        [Hidden]
        public int? NextTaskId { get; init; }

        [MemberOrder(120)]
        public virtual Task NextTask { get; init; }

        [MemberOrder(130)]
        public bool NextTaskClearsFunctions { get; init; }

        [RenderEagerly]
        public virtual ICollection<Hint> Hints { get; set; } = new List<Hint>();

        public override string ToString() => $"{Title} ({Language})";
    }
}