namespace Model.Types
{
    [Bounded]
    public class Organisation
    {
        public Organisation() { }

        public Organisation(Organisation cloneFrom) { 
            Id = cloneFrom.Id;
            Name = cloneFrom.Name;
        }

        [Hidden]
        public int Id { get; init; }

        public string Name { get; init; }

        public virtual ICollection<User> Teachers { get; set; } = new List<User>();

        public override string ToString() => Name;
    }
}
