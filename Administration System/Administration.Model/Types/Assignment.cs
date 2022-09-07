

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

        [MemberOrder(1)]
        public DateTime DueBy { get; init; }

        [MemberOrder(2)]
        public AssignmentStatus Status { get; init; }

        [Hidden]
        public int TaskId { get; init; }
        [MemberOrder(3)]
        public virtual Task Task { get; init; }

        [MemberOrder(12)]
        public int MarksAwarded { get; init; }

        [Hidden]
        public int AssignedToId { get; init; }
        [MemberOrder(4)]
        public virtual Student AssignedTo { get; init; }

        [Hidden]
        public int AssignedById { get; init; }
        [MemberOrder(14)]
        public virtual Teacher AssignedBy { get; init; }

        public override string ToString() => $"{Task}";
    }
}
