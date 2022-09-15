namespace Model.Types
{
    public class User
    {
        public User() { }

        public User(User cloneFrom)
        {
            Id = cloneFrom.Id;
            UserName = cloneFrom.UserName;
            Role = cloneFrom.Role;
            Name = cloneFrom.Name;
            EmailAddress = cloneFrom.EmailAddress;
            OrganisationId = cloneFrom.OrganisationId;
            Organisation = cloneFrom.Organisation;
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(1)]
        public string UserName { get; init; }

        [MemberOrder(2)]
        public Role Role { get; init; }

        [MemberOrder(2)]
        public string Name { get; init; }

        [MemberOrder(3)]
        public string EmailAddress { get; init; }

        [MemberOrder(4)]
        public MemberStatus Status { get; init; }

        [Hidden]
        public int OrganisationId { get; init; }
        [MemberOrder(6)]
        public virtual Organisation Organisation { get; init; }

        public override string ToString() => $"{Name}{(Status == MemberStatus.PendingAcceptance ? " (PENDING)" : null)}";
    }


}
