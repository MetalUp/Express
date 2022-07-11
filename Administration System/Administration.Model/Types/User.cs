namespace Model.Types
{
    public class User
    {
        public User() { }

        public User(User cloneFrom)
        {
            Id = cloneFrom.Id;
            FriendlyName = cloneFrom.FriendlyName;
            UserName = cloneFrom.UserName;
            Password = cloneFrom.Password;
        }

        [Hidden]
        public int Id { get; init; }

        public string FriendlyName { get; init; }

        public string UserName { get; init; }

        public Role Role { get; init; }

        public virtual Organisation Organisation { get; init; }


        [Hidden]
        public string Password { get; init; }


        public override string ToString() => FriendlyName;
    }

    public enum Role
    {
        User, Student, Teacher, Author, System
    }
}
