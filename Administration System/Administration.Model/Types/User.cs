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

        public string FullName { get; init; }

        //[RegEx()]
        public string UserName { get; init; }

        public Role Role { get; init; }

        [Hidden]
        public string Password { get; init; }

        [Hidden]
        public int? OrganisationId { get; init; }
        public virtual Organisation Organisation { get; init; }

        public ProgrammingLanguage PreferredLanguage { get; init; }

        public override string ToString() => FullName;
    }


}
