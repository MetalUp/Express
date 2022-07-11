

namespace Model.Types
{
    public class Assignment
    {
        public Assignment() { }
        public Assignment(Assignment cloneFrom) { 
        Id = cloneFrom.Id;
            Task = cloneFrom.Task;
            AssignedTo = cloneFrom.AssignedTo;
            AssignedBy  = cloneFrom.AssignedBy;
            DueBy = cloneFrom.DueBy;
            Status = cloneFrom.Status;
            MarksAwarded = cloneFrom.MarksAwarded;
            Status = cloneFrom.Status;
        }
        [Hidden]
        public int Id { get; init; }

        public virtual Task Task { get; init; }

        public virtual User AssignedTo { get; init; }

        public virtual Group Group { get; init; } // optional, to allow retrieval of tasks assigned to groups without showing all individuals

        public virtual User AssignedBy { get; init; }

        public DateTime DueBy { get; init; }

        public Activity Status {get; init; }

        public int MarksAwarded { get; init; }

        public override string ToString() => $"{Task}";
    }
}
