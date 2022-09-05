namespace Model.Types
{
    public class Task
    {
        public Task() { }
        public Task(Task cloneFrom)
        {
            Id = cloneFrom.Id;
            Name = cloneFrom.Name;
        }

        [Hidden]
        public int Id { get; init; }

        public string Name { get; init; }

        public override string ToString() => Name;
    }
}
