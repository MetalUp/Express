

namespace Model.Types
{
    public class Assignment
    {
        public Assignment() { }
        public Assignment(Assignment cloneFrom) { 
            Id = cloneFrom.Id;
            ProjectId = cloneFrom.ProjectId;
            Project = cloneFrom.Project;
            AssignedToId = cloneFrom.AssignedToId;
            AssignedTo = cloneFrom.AssignedTo;
            AssignedById = cloneFrom.AssignedById;
            AssignedBy  = cloneFrom.AssignedBy;
            DueBy = cloneFrom.DueBy;
            Status = cloneFrom.Status;
            Marks = cloneFrom.Marks;
        }
        [Hidden]
        public int Id { get; init; }

        [MemberOrder(0)][UrlLink("Start or continue Project")]
        public string Link =>  $"https://express.metalup.org/task/{Project.Tasks.First().Id}";

        [MemberOrder(1)]
        public DateTime DueBy { get; init; }

        [MemberOrder(2)]
        public AssignmentStatus Status { get; init; }

        [Hidden]
        public int ProjectId { get; init; }
        [MemberOrder(3)]
        public virtual Project Project { get; init; }

        [MemberOrder(12)]
        public int Marks { get; init; }

        [Hidden]
        public int AssignedToId { get; init; }
        [MemberOrder(4)]
        public virtual User AssignedTo { get; init; }

        [Hidden]
        public int AssignedById { get; init; }
        [MemberOrder(14)]
        public virtual User AssignedBy { get; init; }

        [MemberOrder(16)]
        [MultiLine(10)]
        public string TeacherNotes { get; init; }

        public override string ToString() => $"{Project}";
    }
}
