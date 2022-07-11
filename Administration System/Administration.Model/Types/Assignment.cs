

namespace Model.Types
{
    public class Assignment
    {
        public Assignment() { }
        public Assignment(Assignment cloneFrom) { 
        Id = cloneFrom.Id;
            Task = cloneFrom.Task;
            Student = cloneFrom.Student;
            AssignedBy  = cloneFrom.AssignedBy;
            DueBy = cloneFrom.DueBy;
            Status = cloneFrom.Status;
            MarksGained = cloneFrom.MarksGained;
            Status = cloneFrom.Status;
        }
        [Hidden]
        public int Id { get; init; }

        public virtual Task Task { get; init; }

        public virtual Student Student { get; init; }

        public virtual Teacher AssignedBy { get; init; }

        public DateTime DueBy { get; init; }

        public Activity Status {get; init; }

        public int MarksGained { get; init; }

        public override string ToString() => $"{Task}";
    }
}
