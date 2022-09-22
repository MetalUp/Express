namespace Model.Types
{
    public class Group
    {
        public Group() { }

        public Group(Group cloneFrom) {
            Id = cloneFrom.Id;
            Name = cloneFrom.Name;
            Description = cloneFrom.Description;
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(10)]
        public string Name { get; init; }

        [MemberOrder(20)]
        public string Description { get; init; }

        [Hidden]
        public int OrganisationId { get; init; }
        public virtual Organisation Organisation { get; init; }         

        public override string ToString() => Name;
    }
}
