
namespace Model.Types
{
    public class Hint
    {
        public Hint() { }
        public Hint(Hint cloneFrom)
        {
            Id = cloneFrom.Id;
            TaskId = cloneFrom.TaskId;
            Task = cloneFrom.Task;
            HtmlFile = cloneFrom.HtmlFile;
            CostInMarks = cloneFrom.CostInMarks;
        }
        [Hidden]
        public int Id { get; init; }

        [MemberOrder(2)]
        public string Title { get; init; }

        [MemberOrder(4)]
        public string HtmlFile { get; init; }

        [MemberOrder(6)]
        public int CostInMarks { get; init; }

        [Hidden]
        public int TaskId { get; init; }
        [MemberOrder(8)]
        public virtual Task Task { get; init; }

        public override string ToString() => $"{Title} ({CostInMarks} marks)";
    }
}
