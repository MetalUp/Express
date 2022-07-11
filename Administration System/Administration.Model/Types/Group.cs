namespace Model.Types
{
    public class Group
    {
        public Group() { }

        public Group(Group cloneFrom) {
            Id = cloneFrom.Id;
            GroupName = cloneFrom.GroupName;
            Description = cloneFrom.Description;
            Teacher = cloneFrom.Teacher;
            Students = cloneFrom.Students;
        }

        [Hidden]
        public int Id { get; init; }

        public string GroupName { get; init; }

        public string Description { get; init; }

        public virtual Teacher Teacher {get; init;}

        public virtual ICollection<Student> Students { get; init; } = new List<Student>();

        public virtual ICollection<Teacher> Teachers { get; init; } = new List<Teacher>();

        public override string ToString() => GroupName;
    }
}
