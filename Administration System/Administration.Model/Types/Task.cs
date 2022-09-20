using NakedFramework.Value;

namespace Model.Types
{
    public class Task
    {
        public Task() { }
        public Task(Task cloneFrom)
        {
            Id = cloneFrom.Id;
            AuthorId = cloneFrom.AuthorId;
            Author = cloneFrom.Author;
            Status = cloneFrom.Status;
            Title = cloneFrom.Title;
            Language = cloneFrom.Language;
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
            TestsMime= cloneFrom.TestsMime;
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
        public int AuthorId { get; init; }
        [Hidden]
        public virtual User Author { get; init; }

        [MemberOrder(0)]
        public TaskStatus Status { get; init; }

        [MemberOrder(1)]
        public string Title { get; init; }

        [MemberOrder(2)]
        public ProgrammingLanguage Language { get; init; }

        //Marks awarded for completing the task with no hints taken
        [MemberOrder(3)]
        public int MaxMarks { get; init; }

        #region Description
        [MemberOrder(5)]  
        public FileAttachment Decription => (DescContent == null) ? null:
                 new FileAttachment(DescContent, DescName, DescMime);

        [Hidden]
        public byte[] DescContent { get; init; }

        [Hidden]
        public string DescName { get; init; }

        [Hidden]
        public string DescMime { get; init; }
        #endregion

        #region ReadyMadeFunctions
        [MemberOrder(6)]
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
        [MemberOrder(7)]
        public FileAttachment Tests => (TestsContent == null) ? null :
                 new FileAttachment(TestsContent, TestsName, TestsMime);

        [Hidden]
        public byte[] TestsContent { get; init; }

        [Hidden]
        public string TestsName { get; init; }

        [Hidden]
        public string TestsMime { get; init; }
        #endregion

        [MemberOrder(9)]
        public bool PasteExpression { get; init; }

        [MemberOrder(9)]
        public bool PasteFunctions { get; init; }

        [Hidden]
        public int? PreviousTaskId { get; init; }

        [MemberOrder(11)]
        public virtual Task PreviousTask { get; init; }

        [Hidden]
        public int? NextTaskId { get; init; }

        [MemberOrder(12)]
        public virtual Task NextTask { get; init; }

        [MemberOrder(13)]
        public bool NextTaskClearsFunctions { get; init; }

        public virtual ICollection<Hint> Hints { get; set; } = new List<Hint>();

        public override string ToString() => $"{Title} ({Language})";
    }
}