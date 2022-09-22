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

        [MemberOrder(3)] //Required only for Teacher role and above
        public string EmailAddress { get; init; }

        [MemberOrder(4)]
        public UserStatus Status { get; init; }

        [Hidden]
        public int OrganisationId { get; init; }
        [MemberOrder(6)]
        public virtual Organisation Organisation { get; init; }

        [MemberOrder(7)]
        public string InvitationCode { get; init; } //Visible only to teachers in organisation and when status is Pending


        public  virtual ICollection<Group> Groups { get; init; } = new List<Group>();

        public override string ToString() => $"{Name} - {(Status == UserStatus.PendingAcceptance ? "PENDING ":null)}{Role}";
    }


}
