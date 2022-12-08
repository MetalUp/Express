namespace Model.Types
{

    public class Organisation
    {
        public Organisation() { }

        public Organisation(Organisation cloneFrom) { 
            Id = cloneFrom.Id;
            Name = cloneFrom.Name;
            Teachers = cloneFrom.Teachers;
            Groups = cloneFrom.Groups;
        }

        [Hidden]
        public int Id { get; init; }

        public string Name { get; init; }

        [MultiLine(10)]
        public string Details { get; init; }

        [Hidden]
        public virtual ICollection<User> Teachers { get; set; } = new List<User>();

        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

        public override string ToString() => Name;
    }
}
