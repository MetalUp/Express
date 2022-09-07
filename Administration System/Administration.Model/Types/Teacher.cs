namespace Model.Types
{
    public class Teacher
    {
        public Teacher() { }
        public Teacher(Teacher cloneFrom) {
            Id = cloneFrom.Id;
            UserId = cloneFrom.UserId;
            User = cloneFrom.User;
            Name = cloneFrom.Name;
            EmailAddress = cloneFrom.EmailAddress;
            OrganisationId = cloneFrom.OrganisationId;
            Organisation = cloneFrom.Organisation;
        }
        [Hidden]
        public int Id { get; init; }

        [Hidden]
        public int UserId { get; init; }
        [MemberOrder(2)]
        public virtual User User { get; init; }

        [MemberOrder(4)]
        public string Name { get; init; }

        [MemberOrder(6)]
        public string EmailAddress { get; init; }

        [Hidden]
        public int OrganisationId { get; init; }
        [MemberOrder(8)]
        public virtual Organisation Organisation { get; init; }

    }
}
