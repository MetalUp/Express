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
        }

        [Hidden]
        public int Id { get; init; }

        [MemberOrder(1)]
        public string UserName { get; init; }

        [MemberOrder(2)]
        public Role Role { get; init; }

        public override string ToString() => $"A {Role} user";
    }


}
