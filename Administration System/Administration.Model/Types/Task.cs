using NakedFramework.Value;

namespace Model.Types
{
    public class Task
    {
        public Task() { }
        public Task(Task cloneFrom)
        {
            Id = cloneFrom.Id;
            Status = cloneFrom.Status;
            AuthorId = cloneFrom.AuthorId;
            Author = cloneFrom.Author;
            Title = cloneFrom.Title;
            Language = cloneFrom.Language;
            DescriptionContent = cloneFrom.DescriptionContent;
            MaxMarks = cloneFrom.MaxMarks;
            ReadyMadeFunctions = cloneFrom.ReadyMadeFunctions;
            PasteExpression = cloneFrom.PasteExpression;
            PasteFunctions = cloneFrom.PasteFunctions;
            Tests = cloneFrom.Tests; 
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


        [MemberOrder(3)]  
        public FileAttachment Decription => (DescriptionContent == null) ? null:
                 new FileAttachment(DescriptionContent, null, @"text/html");

        [Hidden]
        public byte[] DescriptionContent { get; init; }

        [Hidden]
        public string DescriptionName { get; init; }

        [Hidden]
        public string DescriptionMime { get; init; }

        //Marks awarded for completing the task with no hints taken
        [MemberOrder(4)]
        public int MaxMarks { get; init; }

        //Filename for code (in the specified language) for ready-made functions and/or data definitions
        [MemberOrder(7)]
        public string ReadyMadeFunctions { get; init; }

        [MemberOrder(8)]
        public bool PasteExpression { get; init; }

        [MemberOrder(9)]
        public bool PasteFunctions { get; init; }

        //Filename for executable tests written in the language specified
        [MemberOrder(10)]
        public string Tests { get; init; }

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