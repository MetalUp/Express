namespace Model.Types
{
    public class Group
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Group() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Group(Group cloneFrom) {
            Id = cloneFrom.Id;
            Name = cloneFrom.Name;
            Description = cloneFrom.Description;
            OrganisationId = cloneFrom.OrganisationId;
            Organisation = cloneFrom.Organisation;
            Students = new List<User>(cloneFrom.Students);
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
