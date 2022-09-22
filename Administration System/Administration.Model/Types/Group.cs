namespace Model.Types
{
    public class Group
    {
        public Group() { }

        public Group(Group cloneFrom) {
            Id = cloneFrom.Id;
            Name = cloneFrom.Name;
            Description = cloneFrom.Description;
            OrganisationId = cloneFrom.OrganisationId;
            Organisation = cloneFrom.Organisation;
            Students = cloneFrom.Students;
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

        public virtual ICollection<User> Students { get; init; } = new List<User>();

        public override string ToString() => Name;
    }
}
