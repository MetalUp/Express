namespace Model.Types
{
    public class Project
    {
        public Project() { }

        public Project(Project cloneFrom)
        {
            Id = cloneFrom.Id;
            AuthorId = cloneFrom.AuthorId;
            Author = cloneFrom.Author;
            Status = cloneFrom.Status;
            Title = cloneFrom.Title;
            Language = cloneFrom.Language;
            Description = cloneFrom.Description;
            TaskObjects = new List<Task>(cloneFrom.TaskObjects);
        }

        [Hidden]
        public int Id { get; init; }

        [Hidden]
        public int AuthorId { get; init; }
        [Hidden]
        public virtual User Author { get; init; }

        [MemberOrder(5)]
        public ProjectStatus Status { get; init; }

        [MemberOrder(10)]
        public string Title { get; init; }

        [MemberOrder(15)]
        public ProgrammingLanguage Language { get; init; }  

        [MemberOrder(20)]
        [MultiLine(10)]
        public string Description { get; init; }

        public ICollection<TaskAuthorView> Tasks => TaskObjects.Select(t => new TaskAuthorView(t)).ToList();

 
        public virtual ICollection<Task> TaskObjects { get; init; } = new List<Task>();

        public override string ToString() => $"{Title} ({Language})";
    }
}
