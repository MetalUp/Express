namespace Model.Types
{
    public class Teacher
    {
        public Teacher() { }

        public Teacher(Teacher cloneFrom)
        {
            Id = cloneFrom.Id;
            FullName = cloneFrom.FullName;
        }

        [Hidden]
        public int Id { get; init; }

        public string FullName { get; init; }

        public virtual ICollection<Teacher> Teachers { get; init; } = new List<Teacher>();

        public override string ToString() => FullName;
    }
}
