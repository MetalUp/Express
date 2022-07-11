namespace Model.Types
{
    public class Task
    {
        public Task() { }
        public Task(Task cloneFrom)
        {
            Id = cloneFrom.Id;
            Title = cloneFrom.Title;
        }

        [Hidden]
        public int Id { get; init; }

        public string Title { get; init; }

        public override string ToString() => Title;
    }
}
