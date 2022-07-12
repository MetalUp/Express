namespace Model.Types
{
    public class Group
    {
        public Group() { }

        public Group(Group cloneFrom) {
            Id = cloneFrom.Id;
            GroupName = cloneFrom.GroupName;
            Description = cloneFrom.Description;
            Students = cloneFrom.Students;
        }

        [Hidden]
        public int Id { get; init; }

        public string GroupName { get; init; }

        public string Description { get; init; }

        public int OrganisationId { get; init; }
        public virtual Organisation Organisation { get; init; }

        public virtual ICollection<User> Students { get; init; } = new List<User>();

        public override string ToString() => GroupName;
    }
}
