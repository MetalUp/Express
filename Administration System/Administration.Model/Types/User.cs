namespace Model.Types
{
    public class User
    {
        public User() { }

        public User(User cloneFrom)
        {
            Id = cloneFrom.Id;
            FullName = cloneFrom.FullName;
            UserName = cloneFrom.UserName;
            Password = cloneFrom.Password;
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(1)]
        public string UserName { get; init; }

        [MemberOrder(2)]
        public string FullName { get; init; }

        [MemberOrder(3)]
        public Role Role { get; init; }

        [Hidden]
        public string Password { get; init; }

        [MemberOrder(4)]
        public ProgrammingLanguage? PreferredLanguage { get; init; }

        [Hidden]
        public int? OrganisationId { get; init; }

        [MemberOrder(5)]
        public virtual Organisation Organisation { get; init; }

        public override string ToString() => $"{UserName} ({FullName})";
    }


}
