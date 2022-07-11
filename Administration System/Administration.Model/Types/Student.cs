namespace Model.Types
{
    public class Student
    {
        public Student() { }

        public Student(Student cloneFrom) { 
            Id = cloneFrom.Id;
            FullName = cloneFrom.FullName;
            Groups = cloneFrom.Groups;
        }

        [Hidden]
        public int Id { get; init; }

        public string FullName { get; init; }

        public virtual ICollection<Group>  Groups { get; init; } = new List<Group>();   

        public override string ToString() => FullName;
    }
}
