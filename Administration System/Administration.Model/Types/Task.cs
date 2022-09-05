namespace Model.Types
{
    public class Task
    {
        public Task() { }
        public Task(Task cloneFrom)
        {
            Id = cloneFrom.Id;
            Language = cloneFrom.Language;
            Title = cloneFrom.Title;
            Description = cloneFrom.Description;
            MaxMarks = cloneFrom.MaxMarks;
            Hints = cloneFrom.Hints;
            HintCosts = cloneFrom.HintCosts;
            ReadyMadeFunctions = cloneFrom.ReadyMadeFunctions;
            PasteExpression = cloneFrom.PasteExpression;
            PasteFunctions = cloneFrom.PasteFunctions;
            Tests = cloneFrom.Tests; 
            PreviousTaskId = cloneFrom.PreviousTaskId;
            PreviousTask = cloneFrom.PreviousTask;
            NextTaskId = cloneFrom.NextTaskId;
            NextTask = cloneFrom.NextTask;
            NextTaskDoesNotClearFunctions = cloneFrom.NextTaskDoesNotClearFunctions;
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(1)]
        public string Title { get; init; }

        [MemberOrder(2)]
        public ProgrammingLanguage Language { get; init; }

        //.html file path
        [MemberOrder(3)]
        public string Description { get; init; }

        //Marks awarded for completing the task with no hints taken
        [MemberOrder(4)]
        public int? MaxMarks { get; init; }

        //Comma-separated .html file paths
        [MemberOrder(5)]
        public string Hints { get; init; }

        //Comma-separated list of integer values representing cost of each hint
        [MemberOrder(6)]
        public string HintCosts { get; init; }

        //File path to code for ready-made functions and/or data definitions
        [MemberOrder(7)]
        public string ReadyMadeFunctions { get; init; }

        [MemberOrder(8)]
        public bool PasteExpression { get; init; }

        [MemberOrder(9)]
        public bool PasteFunctions { get; init; }

        //File path to code for executable tests
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
        public bool NextTaskDoesNotClearFunctions { get; init; }

        public override string ToString() => $"{Title} ({Language})";
    }
}